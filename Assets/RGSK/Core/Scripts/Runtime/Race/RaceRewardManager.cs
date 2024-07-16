using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceRewardManager : Singleton<RaceRewardManager>
    {
        public RaceReward Reward => _reward;

        RaceReward _reward;
        RaceSession _session;

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceDeInitialized.AddListener(OnRaceDeInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceDeInitialized.RemoveListener(OnRaceDeInitialized);
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (c.Entity.IsPlayer)
            {
                _session = RaceManager.Instance.Session;

                SaveSessionRecord(c);
                GiveRewards(c.FinalPosition - 1);

                SaveData.Instance.playerData.totalRaces += 1;

                if (c.FinalPosition == 1)
                {
                    SaveData.Instance.playerData.totalWins += 1;
                }
            }
        }

        void OnRaceDeInitialized()
        {
            if (_reward != null)
            {
                _reward = null;
            }
        }

        void SaveSessionRecord(Competitor c)
        {
            if (!_session.saveRecords || c.IsDisqualified)
                return;

            if (_session.UseTargetScores() && c.FinalPosition > _session.targetScores.Count)
                return;

            var record = _session.LoadBestPosition();
            if (record == 0 || record > c.FinalPosition)
            {
                _session.SaveBestPosition(c.FinalPosition);
            }
        }

        void GiveRewards(int rewardIndex)
        {
            if (_session.raceRewards.Count <= rewardIndex)
                return;

            _reward = _session.raceRewards[rewardIndex];

            _reward.items.ForEach(x => x?.Unlock());

            SaveData.Instance.playerData.currency += _reward.currency;
            SaveData.Instance.playerData.xp += _reward.xp;
        }
    }
}