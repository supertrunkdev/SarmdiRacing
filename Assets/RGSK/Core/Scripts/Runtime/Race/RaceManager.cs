using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceManager : Singleton<RaceManager>
    {
        public RaceSession Session => _session;
        public Track Track => _track;
        public RaceState CurrentState { get; private set; }
        public EntityRuntimeSet Entities => RGSKCore.Instance.GeneralSettings.entitySet;
        public bool Initialized => _initialized;
        public bool InfiniteLaps => _infiniteLaps;

        RaceSession _session;
        Track _track;
        RaceSpawner _spawner;
        RacePositioningSystem _positioningSystem;
        Timer _raceTimer;
        Timer _sessionTimer;
        DnfTimer _dnfTimer;
        Dictionary<int, Timer> _sessionTimers = new Dictionary<int, Timer>();
        bool _initialized;
        bool _infiniteLaps;

        void OnEnable()
        {
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
        }

        void OnDisable()
        {
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
        }

        public void InitializeRace(RaceSession session, Track track)
        {
            if (Initialized)
                return;

            if (session == null || track == null)
            {
                Logger.LogWarning(this, "A race session and a track are required to initialize the race!");
                return;
            }

            _session = ScriptableObject.Instantiate(session);
            _track = track;

            Session.PopulateOpponents();
            Session.Setup();
            track.Setup();

            if (track.IsPointToPoint())
            {
                Session.lapCount = 1;
            }

            _positioningSystem = gameObject.GetOrAddComponent<RacePositioningSystem>();
            _positioningSystem.PositioningMode = Session.GetPositioningMode();
            _positioningSystem.Sort = Session.raceType.positionSortMode;

            _raceTimer = GeneralHelper.CreateTimer(0, false, false, null, null, null, null, "Timer_Race");
            _dnfTimer = gameObject.GetOrAddComponent<DnfTimer>();

            _infiniteLaps = Session.IsInfiniteLaps();

            if (Session.UseGlobalTimer())
            {
                _sessionTimer = GeneralHelper.CreateTimer(Session.sessionTimeLimit, true,
                    Session.raceType.globalTimerElapsedAction == GlobalTimerElapsedAction.DisqualifyLastPlace,
                    () => OnSessionTimerElapsed(),
                    null,
                    null,
                    null,
                    "Timer_Session");
            }

            _spawner = gameObject.GetOrAddComponent<RaceSpawner>();

            _initialized = true;
            RGSKEvents.OnRaceInitialized.Invoke();
            SetState(RaceState.PreRace);
        }

        public void DeInitializeRace()
        {
            if (!Initialized)
                return;

            _session = null;
            _track = null;

            Destroy(_spawner);
            Destroy(_positioningSystem);
            _sessionTimers.Clear();

            if (_raceTimer != null)
            {
                Destroy(_raceTimer.gameObject);
            }

            if (_dnfTimer != null)
            {
                Destroy(_dnfTimer);
            }

            if (_sessionTimer != null)
            {
                Destroy(_sessionTimer.gameObject);
            }

            foreach (var e in Entities.Items)
            {
                Destroy(e.gameObject);
            }

            SaveManager.Instance?.Save();
            _initialized = false;
            RGSKEvents.OnRaceDeInitialized.Invoke();
        }

        public void StartRace()
        {
            if (CurrentState == RaceState.PreRace)
            {
                UIScreenFader.Instance?.FadeOut(0.5f);

                switch (Session.startMode)
                {
                    case RaceStartMode.StandingStart:
                        {
                            foreach (var c in Entities.Items)
                            {
                                GeneralHelper.ToggleInputControl(c.gameObject, false);
                            }

                            StartCountdown();
                            break;
                        }

                    case RaceStartMode.RollingStart:
                        {
                            foreach (var c in Entities.Items)
                            {
                                GeneralHelper.ToggleAIInput(c.gameObject, true);
                                GeneralHelper.TogglePlayerInput(c.gameObject, false);
                                GeneralHelper.ToggleInputControl(c.gameObject, true);
                            }

                            SetState(RaceState.RollingStart);
                            Invoke(nameof(StartCountdown), RGSKCore.Instance.RaceSettings.rollingStartCountdownWait);
                            break;
                        }
                }
            }
            else
            {
                Entities.Items.ForEach(x => StartRace(x));
            }
        }

        public void StartRace(RGSKEntity entity)
        {
            GeneralHelper.ToggleAIInput(entity.gameObject, !entity.IsPlayer);
            GeneralHelper.TogglePlayerInput(entity.gameObject, entity.IsPlayer);
            GeneralHelper.ToggleInputControl(entity.gameObject, true);
            GeneralHelper.SetAIBehaviour(entity.gameObject, RaceManager.Instance.Session.GetAiDifficulty());
            GeneralHelper.ToggleVehicleCollision(entity.gameObject, !Session.disableCollision);
            GeneralHelper.ToggleGhostedMesh(entity.gameObject, !entity.IsPlayer && Session.disableCollision);

            if (Session.UseSeparateTimers())
            {
                if (!_sessionTimers.ContainsKey(entity.ID))
                {
                    _sessionTimers.Add(entity.ID, GeneralHelper.CreateTimer(
                                Session.sessionTimeLimit,
                                true, false,
                                () => Disqualify(entity.Competitor),
                                null,
                                null,
                                null,
                                $"Timer_{entity.name}"));
                }
            }

            _sessionTimers.FirstOrDefault(x => x.Key == entity.ID).Value?.StartTimer();
            _raceTimer?.StartTimer();
            _sessionTimer?.StartTimer();

            entity.Competitor?.UpdateLapStartTime();
            entity.Competitor?.SetState(RaceState.Racing);

            if (entity.DriftController != null)
            {
                entity.DriftController.enabled = Session.raceType.vehicleHandlingMode == VehicleHandlingMode.Drift;
            }

            //Set an initial offset for the AI to avoid moving to the racing line once the race starts
            if (entity.TryGetComponent<AIController>(out var ai))
            {
                var duration = Track?.raceStartOffsetDuration ?? -1;
                ai.SetTravelOffset(ai.GetLateralPositionFromCenter(ai.transform.position).x, duration);
            }

            if (AllCompetitorsStarted())
            {
                SetState(RaceState.Racing);
            }

            RGSKEvents.OnCompetitorStarted.Invoke(entity.Competitor);
        }

        public void StartCountdown()
        {
            if (!Initialized)
                return;

            SetState(RaceState.Countdown);
        }

        public void RestartRace()
        {
            if (!Initialized)
                return;

            _raceTimer?.ResetTimer();
            _sessionTimers.Values.ToList().ForEach(x => x?.ResetTimer());
            _sessionTimer?.ResetTimer();

            foreach (var entity in Entities.Items)
            {
                _spawner?.PlaceOnGrid(entity.Competitor);

                if (!entity.IsVirtual)
                {
                    entity.Competitor?.ResetValues();
                    entity.DriftController?.ResetValues();
                    entity.Competitor?.SetState(RaceState.PreRace);
                }

                entity.Competitor?.UpdateLapStartTime();
                GeneralHelper.ToggleAIInput(entity.gameObject, !entity.IsPlayer);
                GeneralHelper.TogglePlayerInput(entity.gameObject, entity.IsPlayer);
                GeneralHelper.ToggleInputControl(entity.gameObject, false);
            }

            SetState(RaceState.PreRace);
            RGSKEvents.OnRaceRestart.Invoke();
        }

        public void FinishRace(Competitor c)
        {
            if (c.IsFinished())
                return;

            GeneralHelper.ToggleAIInput(c.gameObject, true);
            GeneralHelper.TogglePlayerInput(c.gameObject, false);

            c.SetState(RaceState.PostRace);
            _sessionTimers.FirstOrDefault(x => x.Key == c.Entity.ID).Value?.StopTimer();
            _positioningSystem.UpdatePositions();
            c.FinalPosition = c.Position;

            if (c.Entity.IsPlayer)
            {
                AudioManager.Instance?.Play(!c.IsDisqualified ?
                                        RGSKCore.Instance.RaceSettings.raceFinishSound :
                                        RGSKCore.Instance.RaceSettings.raceDisqualifySound,
                                        AudioGroup.Music);

                SetState(RaceState.PostRace);
            }

            if (AllCompetitorsFinished())
            {
                _sessionTimer?.StopTimer();
                _raceTimer?.StopTimer();
                _positioningSystem.enabled = false;

                if (CurrentState != RaceState.PostRace)
                {
                    SetState(RaceState.PostRace);
                }
            }

            RGSKEvents.OnCompetitorFinished.Invoke(c);
        }

        public void Disqualify(Competitor c)
        {
            if (c.IsFinished() || c.IsDisqualified)
                return;

            c.IsDisqualified = true;
            GeneralHelper.ToggleVehicleCollision(c.gameObject, false);
            GeneralHelper.ToggleGhostedMesh(c.gameObject, !c.Entity.IsPlayer && RGSKCore.Instance.RaceSettings.ghostDisqualifiedCompetitors);
            FinishRace(c);

            if (ActiveCompetitorCount() == 1 || c.Entity.IsPlayer)
            {
                ForceFinishRace(false);
            }
        }

        public void ForceFinishRace(bool forceDQ)
        {
            foreach (var entity in Entities.Items.Where(x => x.Competitor != null && x.Competitor.IsRacing()))
            {
                if (forceDQ)
                {
                    Disqualify(entity.Competitor);
                }
                else
                {
                    FinishRace(entity.Competitor);
                }
            }
        }

        void OnSessionTimerElapsed()
        {
            switch (Session.raceType.globalTimerElapsedAction)
            {
                case GlobalTimerElapsedAction.Finish:
                    {
                        ForceFinishRace(false);
                        break;
                    }

                case GlobalTimerElapsedAction.DisqualifyLastPlace:
                    {
                        DisqualifyLastPlace();
                        break;
                    }

                case GlobalTimerElapsedAction.FinalLap:
                    {
                        var finalLap = GetCompetitorInPosition(1).CurrentLap;
                        Session.lapCount = finalLap;
                        _infiniteLaps = false;
                        foreach (var entity in Entities.Items)
                        {
                            if (entity.Competitor != null)
                            {
                                entity.Competitor.TotalLaps = finalLap;
                                RGSKEvents.OnLapCompleted.Invoke(entity.Competitor);
                            }
                        }
                        break;
                    }
            }
        }

        void OnLapCompleted(Competitor c)
        {
            if (Session.raceType.disqualifyLastPlaceEachLap)
            {
                if (c.Position == ActiveCompetitorCount() - 1)
                {
                    DisqualifyLastPlace();
                }
            }

            if (!_infiniteLaps && c.CurrentLap > c.TotalLaps)
            {
                FinishRace(c.Entity.Competitor);
            }

            if (c.Entity.IsPlayer && Track.definition != null)
            {
                var record = Track.GetBestLap();
                var best = c.GetBestLapTime();

                if (record == 0 || record > best)
                {
                    Track.definition.SaveBestLap(best);
                    RGSKEvents.OnSetNewBestLapTime.Invoke(c);
                }
            }
        }

        public void ExtendCompetitorTimer(Competitor c, float value)
        {
            if (_sessionTimers.TryGetValue(c.Entity.ID, out var timer))
            {
                timer.AddTimerValue(value);
                RGSKEvents.OnTimerExtended.Invoke(c, value);
            }
        }

        public void AddCompetitorScore(Competitor c, float value)
        {
            c.Score += value;
            RGSKEvents.OnScoreAdded.Invoke(c, value);
        }

        void SetState(RaceState state)
        {
            CurrentState = state;

            switch (state)
            {
                case RaceState.PreRace:
                    {
                        InputManager.Instance?.SetInputMode(InputManager.InputMode.Disabled);
                        _positioningSystem.enabled = true;

                        if (RGSKCore.Instance.RaceSettings.skipPreRaceState)
                        {
                            StartRace();
                        }
                        break;
                    }

                case RaceState.Countdown:
                    {
                        InputManager.Instance?.SetInputMode(InputManager.InputMode.Gameplay);
                        Entities.Items.ForEach(x => x?.Competitor?.SetState(Session.startMode == RaceStartMode.StandingStart ? RaceState.Countdown : RaceState.RollingStart));
                        break;
                    }

                case RaceState.RollingStart:
                    {
                        InputManager.Instance?.SetInputMode(InputManager.InputMode.Gameplay);
                        Entities.Items.ForEach(x => x?.Competitor?.SetState(RaceState.RollingStart));
                        break;
                    }

                case RaceState.PostRace:
                    {
                        InputManager.Instance?.SetInputMode(InputManager.InputMode.Disabled);
                        break;
                    }
            }

            //enable the cinematic camera based on the race state
            CameraManager.Instance?.ToggleRouteCameras(EnumFlags.GetSelectedIndexes
                                (RGSKCore.Instance.RaceSettings.cinematicCameraStates).
                                Contains((int)state));

            RGSKEvents.OnRaceStateChanged.Invoke(CurrentState);
        }

        void DisqualifyLastPlace()
        {
            var last = GetCompetitorInLastPlace();
            if (last != null)
            {
                Disqualify(last.Entity.Competitor);
            }
        }

        public float GetRaceTime()
        {
            if (_raceTimer == null)
                return 0;

            return _raceTimer.Value;
        }

        public float GetSessionTime()
        {
            if (!Initialized || Session.raceType.raceDurationMode == RaceDurationMode.LapBased)
                return 0;

            switch (Session.raceType.timerMode)
            {
                case RaceTimerMode.Global:
                    {
                        return _sessionTimer.Value;
                    }

                case RaceTimerMode.PerCompetitor:
                    {
                        var c = GeneralHelper.GetFocusedEntity();

                        if (c != null)
                        {
                            if (_sessionTimers.TryGetValue(c.ID, out var timer))
                            {
                                return timer.Value;
                            }
                        }

                        return 0;
                    }
            }

            return 0;
        }

        public float GetDnfTime() => _dnfTimer?.GetRemainingTime() ?? 0;

        public Competitor GetCompetitorInPosition(int pos)
        {
            if (pos < 1 || pos > _positioningSystem.SortedList.Count)
                return null;

            return _positioningSystem.SortedList[pos - 1].Competitor;
        }

        public Competitor GetCompetitorInLastPlace()
            => GetCompetitorInPosition(ActiveCompetitorCount());

        public Competitor GetCompetitorWithBestLap()
            => _positioningSystem.SortedList.OrderBy(x => x.Competitor.GetBestLapTime()).FirstOrDefault()?.Competitor;

        public bool AllCompetitorsStarted()
            => _positioningSystem.SortedList.Where(x => !x.IsVirtual && x.Competitor != null).All(x => x.Competitor.IsRacing());

        public bool AllCompetitorsFinished()
            => _positioningSystem.SortedList.Where(x => !x.IsVirtual && x.Competitor != null).All(x => x.Competitor.IsFinished() || x.Competitor.IsDisqualified);

        public int ActiveCompetitorCount()
            => _positioningSystem.SortedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => !x.Competitor.IsDisqualified);

        public int FinishedCompetitorCount()
            => _positioningSystem.SortedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => x.Competitor.IsFinished() && !x.Competitor.IsDisqualified);

        public int DisqualifiedCompetitorCount()
            => _positioningSystem.SortedList.Where(x => !x.IsVirtual && x.Competitor != null).Count(x => x.Competitor.IsDisqualified);
    }
}