using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class RacePositioningSystem : MonoBehaviour
    {
        public RacePositioningMode PositioningMode { get; set; }
        public SortOrder Sort { get; set; }
        public List<RGSKEntity> SortedList { get; private set; } = new List<RGSKEntity>();

        float _updatesPerSecond = 10;
        float _lastUpdate;

        void Update()
        {
            if (Time.time > _lastUpdate)
            {
                _lastUpdate = Time.time + (1 / _updatesPerSecond);
                UpdatePositions();
            }
        }

        public void UpdatePositions()
        {
            SortedList = RaceManager.Instance.Entities.Items.OrderBy(x => GetRankableValue(x)).ToList();

            if (Sort == SortOrder.Descending)
            {
                SortedList.Reverse();
            }

            for (int i = 0; i < SortedList.Count; i++)
            {
                var c = SortedList[i].Competitor;

                if (c != null && c.Position != (i + 1))
                {
                    c.SetPosition(i + 1);
                }
            }
        }

        float GetRankableValue(RGSKEntity e)
        {
            if (e.Competitor == null)
                return 0;

            switch (PositioningMode)
            {
                case RacePositioningMode.Distance:
                default:
                    {
                        if (e.Competitor.IsDisqualified)
                        {
                            return int.MinValue + e.Competitor.DistanceTravelled;
                        }

                        if (e.Competitor.FinalPosition > 0 && e.Competitor.TotalLaps > 0)
                        {
                            return (e.Competitor.TotalRaceDistance / e.Competitor.FinalPosition) * e.Competitor.TotalRaceDistance;
                        }

                        if (e.Competitor.TotalCheckpointsPassed == 0 && e.Competitor.NextCheckpoint != null)
                        {
                            return -Vector3.Distance(e.Competitor.transform.position,
                                                     e.Competitor.NextCheckpoint.transform.position);
                        }

                        return e.Competitor.DistanceTravelled;
                    }

                case RacePositioningMode.DriftPoints:
                    {
                        return e.DriftController.TotalPoints;
                    }

                case RacePositioningMode.TotalSpeed:
                    {
                        return e.Competitor.TotalSpeedtrapSpeed;
                    }

                case RacePositioningMode.AverageSpeed:
                    {
                        return e.Competitor.AverageSpeed;
                    }

                case RacePositioningMode.Score:
                    {
                        return e.Competitor.Score;
                    }

                case RacePositioningMode.TotalTime:
                    {
                        return e.Competitor.TotalRaceTime;
                    }

                case RacePositioningMode.BestLapTime:
                    {
                        return e.Competitor.GetBestLapTime();
                    }
            }
        }
    }
}