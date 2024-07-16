using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class RaceSpawner : MonoBehaviour
    {
        List<Transform> _gridPositions = new List<Transform>();
        List<ProfileDefinition> _opponentProfiles;
        Track _track;
        int _opponentCount = 1;

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
        }

        void OnRaceInitialized()
        {
            var session = RaceManager.Instance.Session;

            Setup(session);

            if (session.UseTargetScores() && session.entrants.Count == 1)
            {
                foreach (var score in session.targetScores)
                {
                    CreateVirtualCompetitor(score);
                }
            }
        }

        public void Setup(RaceSession session)
        {
            _track = RaceManager.Instance.Track;
            var grid = _track?.GetRaceGrid(session.startMode)?.GetPositions();
            var maxSlots = session.entrants.Count;

            if (grid == null)
            {
                Logger.LogWarning(this, $"No starting grid [{session.startMode}] was found! Please create a starting grid [{session.startMode}] for the track.");
                return;
            }

            _gridPositions = new List<Transform>(grid);

            if (_gridPositions.Count > 0 && _gridPositions.Count >= maxSlots)
            {
                _gridPositions.RemoveRange(maxSlots, _gridPositions.Count - maxSlots);
            }

            SpawnEntrants(session);
        }

        void SpawnEntrants(RaceSession session)
        {
            if (session.entrants.Count == 0)
                return;

            if (session.entrants.Count > _gridPositions.Count)
            {
                Logger.LogWarning(this, $"There are not enough spawn points for all the entrants! You are trying to spawn {session.entrants.Count} entrans but only have {_gridPositions.Count} grid positions!");
                return;
            }

            var track = RaceManager.Instance.Track;
            var player = session.entrants.FirstOrDefault(x => x.isPlayer);

            if (player != null)
            {
                session.playerStartPosition = Mathf.Clamp(session.playerStartPosition, 1, session.entrants.Count);

                switch (session.playerGridStartMode)
                {
                    case SelectionMode.Random:
                        {
                            session.entrants.Move(session.entrants.IndexOf(player), Random.Range(0, session.entrants.Count - 1));
                            break;
                        }

                    case SelectionMode.Selected:
                        {
                            session.entrants.Move(session.entrants.IndexOf(player), session.playerStartPosition - 1);
                            break;
                        }
                }
            }

            for (int i = 0; i < session.entrants.Count; i++)
            {
                if (session.entrants[i].prefab == null)
                    continue;

                var isPlayer = session.entrants[i].isPlayer;
                var instance = Instantiate(session.entrants[i].prefab, GeneralHelper.GetDynamicParent());
                var entity = instance.GetOrAddComponent<RGSKEntity>();
                var profile = instance.GetOrAddComponent<ProfileDefiner>();
                var competitor = instance.GetOrAddComponent<Competitor>();
                var ai = instance.GetOrAddComponent<AIController>();
                var driftController = instance.GetOrAddComponent<DriftController>();
                instance.GetOrAddComponent<Slipstream>();
                instance.GetOrAddComponent<RecordableSceneObject>();

                if (isPlayer && session.UseGhostVehicle() && session.enableGhost)
                {
                    instance.GetOrAddComponent<RecordableGhostObject>();
                }

                profile.definition = session.entrants[i].profile == null ?
                                        isPlayer ? RGSKCore.Instance.GeneralSettings.playerProfile :
                                        GetOpponentProfile() :
                                        session.entrants[i].profile;

                competitor.StartingPosition = i;
                competitor.SetState(RaceState.PreRace);
                competitor.Setup(track?.CheckpointRoute, RaceManager.Instance.InfiniteLaps ? -1 : session.lapCount);

                if (instance.TryGetComponent<IVehicle>(out var v))
                {
                    v.Initialize();
                    v.StartEngine(0);
                }

                entity.Initialize(false);
                entity.IsPlayer = isPlayer;

                ai.ToggleActive(false);

                if (RGSKCore.Instance.UISettings.nameplate != null)
                {
                    var nameplate = Instantiate(RGSKCore.Instance.UISettings.nameplate, GeneralHelper.GetDynamicParent());
                    nameplate.autoBindToFocusedEntity = false;
                    nameplate.BindElements(entity);
                }

                GeneralHelper.ToggleGhostedMesh(instance, false);

                if (GeneralHelper.CanApplyColor(instance))
                {
                    if (session.entrants[i].colorSelectMode == ColorSelectionMode.Random)
                    {
                        session.entrants[i].color = GeneralHelper.GetRandomVehicleColor();
                    }

                    GeneralHelper.SetColor(instance, session.entrants[i].color);
                }
                else if (GeneralHelper.CanApplyLivery(instance))
                {
                    if (session.entrants[i].colorSelectMode == ColorSelectionMode.Random)
                    {
                        GeneralHelper.SetRandomLivery(instance);
                    }
                    else if (session.entrants[i].colorSelectMode == ColorSelectionMode.Livery)
                    {
                        GeneralHelper.SetLivery(instance, session.entrants[i].livery);
                    }
                }

                GeneralHelper.SetHandlingMode(instance, session.raceType.vehicleHandlingMode);
                GeneralHelper.TogglePlayerInput(instance, isPlayer);
                GeneralHelper.ToggleAIInput(instance, !isPlayer);
                GeneralHelper.ToggleInputControl(instance, false);

                MinimapManager.Instance?.CreateBlip(isPlayer ?
                                                RGSKCore.Instance.UISettings.playerMinimapBlip :
                                                RGSKCore.Instance.UISettings.opponentMinimapBlip,
                                                instance.transform);

                PlaceOnGrid(competitor);
            }
        }

        public void PlaceOnGrid(Competitor c)
        {
            var rb = c.GetComponent<Rigidbody>();
            var ai = c.GetComponent<AIController>();
            var vehicle = c.GetComponent<IVehicle>();

            c.transform.SetPositionAndRotation(_gridPositions[c.StartingPosition].position,
                                               _gridPositions[c.StartingPosition].rotation);

            rb?.ResetVelocity();
            ai?.SetRoute(_track?.GetAIRoute(0));

            if (vehicle != null)
            {
                vehicle.OnReset();
                vehicle.Repair();
                vehicle.HeadlightsOn = false;
            }

            if (c.Entity.IsPlayer)
            {
                CameraManager.Instance?.SetTarget(c.transform);
            }
        }

        void CreateVirtualCompetitor(float score)
        {
            var entity = new GameObject($"[Virtual Competitor]").AddComponent<RGSKEntity>();
            entity.transform.SetParent(GeneralHelper.GetDynamicParent(), false);
            entity.gameObject.AddComponent<Competitor>();
            entity.gameObject.AddComponent<DriftController>();
            entity.Initialize(true);

            entity.DriftController.TotalPoints = (int)score;
            entity.Competitor.TotalRaceTime = score;
            entity.Competitor.TotalSpeedtrapSpeed = score;
            entity.Competitor.AverageSpeed = score;
            entity.Competitor.Score = score;

            entity.Competitor.SetState(RaceState.PostRace);
        }

        ProfileDefinition GetOpponentProfile()
        {
            if (_opponentProfiles == null)
            {
                _opponentProfiles = RGSKCore.Instance.AISettings.opponentProfiles.ToList();
            }

            if (_opponentProfiles.Count > 0)
            {
                var p = _opponentProfiles.GetRandom();
                _opponentProfiles.Remove(p);
                return p;
            }
            else
            {
                var p = GeneralHelper.CreateProfileDefinition($"AI {_opponentCount}");

                if (RGSKCore.Instance.GeneralSettings.countrySettings != null)
                {
                    p.nationality = RGSKCore.Instance.GeneralSettings.countrySettings.countries.GetRandom();
                }

                _opponentCount++;
                return p;
            }
        }
    }
}