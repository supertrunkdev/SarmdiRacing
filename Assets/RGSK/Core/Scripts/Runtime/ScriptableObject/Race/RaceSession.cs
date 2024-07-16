using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class RaceEntrant
    {
        public GameObject prefab;
        public ProfileDefinition profile;
        public Color color = Color.white;
        public Texture2D livery;
        public ColorSelectionMode colorSelectMode;
        public bool isPlayer;
    }

    [System.Serializable]
    public class RaceReward
    {
        public List<ItemDefinition> items = new List<ItemDefinition>();
        public int currency;
        public int xp;
    }

    [CreateAssetMenu(menuName = "RGSK/Race/Race Session")]
    public class RaceSession : ItemDefinition
    {
        public RaceSessionType sessionType;

        [Tooltip("The race type used for this session.")]
        public RaceType raceType;

        [Tooltip("The way the race should start.")]
        public RaceStartMode startMode;

        [Tooltip("The number of laps in the race.\nThis is automatically set to 1 for point to point tracks.")]
        public int lapCount = 3;

        [Tooltip("The difficulty index of the AI opponents.")]
        public int opponentDifficulty;

        [Tooltip("The number of opponents to use when auto populating opponnets.")]
        public int opponentCount = 3;

        public SelectionMode playerGridStartMode;
        public int playerStartPosition = 0;

        [Tooltip("The value that the session timer(s) will start at.\nOnly available in race types that are timed.")]
        public float sessionTimeLimit = 60;

        [Tooltip("Whether a ghost vehicle will be used.\nOnly available in race types that allow ghosts.")]
        public bool enableGhost = true;

        [Tooltip("Disables collision between competitors in the race.")]
        public bool disableCollision;

        [Tooltip("The race entrants")]
        public List<RaceEntrant> entrants = new List<RaceEntrant>();

        [Tooltip("The scores that the player has to achieve in the race session.")]
        public List<float> targetScores = new List<float>();

        [Tooltip("Should the opponents be added automatically.")]
        public bool autoPopulateOpponents;
        public OpponentClassOptions opponentClass = OpponentClassOptions.SameAsPlayer;
        [Tooltip("The vehicle class the opponents will be populated from.\nLeave null to use all vehicle classes.")]
        public VehicleClass opponentVehicleClass;

        public List<RaceReward> raceRewards = new List<RaceReward>();
        public TrackDefinition track;

        public bool saveRecords = true;

        public void Setup()
        {
            if (raceType == null)
            {
                Logger.LogWarning("A race type has not been assigned to this session! Default race type will be used.");
                raceType = ScriptableObject.CreateInstance<RaceType>();
            }

            if (raceType.raceDurationMode == RaceDurationMode.TimeBased)
            {
                raceType.infiniteLaps = true;
            }

            if (!IsInfiniteLaps())
            {
                lapCount = Mathf.Clamp(lapCount, raceType.minLaps, lapCount);
            }
        }

        public void PopulateOpponents()
        {
            if (IsSoloSession())
            {
                //remove all opponnents in a solo session
                var opponents = entrants.Where(x => !x.isPlayer).ToList();
                opponents.ForEach(x => entrants.Remove(x));
                return;
            }

            if (autoPopulateOpponents)
            {
                var vehicles = RGSKCore.Instance.ContentSettings.vehicles.ToList();
                var index = 0;

                if (vehicles.Count == 0)
                {
                    Logger.Log("Cannot auto populate opponents because no vehicles found!");
                    return;
                }

                switch (opponentClass)
                {
                    case OpponentClassOptions.SameAsPlayer:
                        {
                            var player = entrants.FirstOrDefault(x => x.isPlayer);
                            if (player != null && player.prefab.TryGetComponent<VehicleDefiner>(out var v))
                            {
                                if (v?.definition?.vehicleClass != null)
                                {
                                    vehicles = RGSKCore.Instance.ContentSettings.vehicles.Where(x => x.vehicleClass == v.definition.vehicleClass).ToList();
                                }
                            }
                            break;
                        }

                    case OpponentClassOptions.Selected:
                        {
                            if (opponentVehicleClass != null)
                            {
                                vehicles = RGSKCore.Instance.ContentSettings.vehicles.Where(x => x.vehicleClass == opponentVehicleClass).ToList();
                            }
                            break;
                        }
                }

                if (vehicles.Count == 0)
                {
                    Logger.LogWarning("No vehicles were found in the class! Oponents will be populated from all vehicle classes.");
                    vehicles = RGSKCore.Instance.ContentSettings.vehicles.ToList();
                }

                vehicles.Shuffle();

                for (int i = 0; i < opponentCount; i++)
                {
                    if (vehicles[index].prefab != null)
                    {
                        entrants.Add(new RaceEntrant
                        {
                            prefab = vehicles[index].prefab,
                            colorSelectMode = ColorSelectionMode.Random,
                            isPlayer = false,
                        });
                    }

                    index = (index + 1) % vehicles.Count;
                }
            }
        }

        public bool IsTimedSession() => UseGlobalTimer() || UseSeparateTimers();

        public bool UseTargetScores() => sessionType == RaceSessionType.TargetScore;

        public RacePositioningMode GetPositioningMode() => raceType?.positioningMode ?? RacePositioningMode.Distance;

        public bool IsSoloSession()
        {
            if (raceType == null)
                return false;

            return raceType.minMaxCompetitorCount.y == 1;
        }

        public bool UseGlobalTimer()
        {
            if (raceType == null)
                return false;

            return raceType.raceDurationMode != RaceDurationMode.LapBased && raceType.timerMode == RaceTimerMode.Global;
        }

        public bool UseSeparateTimers()
        {
            if (raceType == null)
                return false;

            return raceType.raceDurationMode != RaceDurationMode.LapBased && raceType.timerMode == RaceTimerMode.PerCompetitor;
        }

        public bool IsInfiniteLaps()
        {
            if (raceType == null)
                return false;

            return raceType.infiniteLaps || raceType.raceDurationMode == RaceDurationMode.TimeBased;
        }

        public bool UseGhostVehicle()
        {
            if (raceType == null)
                return false;

            return raceType.allowGhost;
        }

        public AIBehaviourSettings GetAiDifficulty()
        {
            if (opponentDifficulty < 0 || opponentDifficulty >= RGSKCore.Instance.AISettings.difficulties.Count)
                return RGSKCore.Instance.AISettings.defaultBehaviour ?? AIBehaviourSettings.CreateDefault();

            return RGSKCore.Instance.AISettings.difficulties[opponentDifficulty];
        }

        public int LoadBestPosition()
        {
            if (!saveRecords)
                return -1;

            if (SaveData.Instance.bestSessionPosition.TryGetValue(ID, out var value))
            {
                return value;
            }

            return 0;
        }

        public void SaveBestPosition(int value)
        {
            if (!saveRecords)
                return;

            SaveData.Instance.bestSessionPosition[ID] = value;
        }
    }
}