using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using RGSK.Extensions;

namespace RGSK
{
    public class RaceSettingsScreen : UIScreen
    {
        [SerializeField] RaceSettingsEntryUI entryPrefab;
        [SerializeField] ScrollRect scrollView;
        [SerializeField] Button startButton;

        [Header("Rewards")]
        [SerializeField] bool autoCalculateRewards = true;
        [SerializeField] int baseCurrencyReward = 200;
        [SerializeField] int baseXpReward = 100;

        [Header("Modal")]
        [SerializeField] ModalWindowProperties modalProperties;
        [SerializeField] bool showModalWindow = true;

        List<RaceSettingsEntryUI> _entries = new List<RaceSettingsEntryUI>();

        public override void Initialize()
        {
            if (entryPrefab != null && scrollView != null)
            {
                foreach (RaceSettingUIType group in Enum.GetValues(typeof(RaceSettingUIType)))
                {
                    var e = Instantiate(entryPrefab, scrollView.content);
                    e.Setup(this, group);
                    _entries.Add(e);
                }
            }

            if (showModalWindow)
            {
                startButton.onClick.AddListener(() =>
                {
                    ModalWindowManager.Instance.Show(new ModalWindowProperties
                    {
                        header = modalProperties.header,
                        message = modalProperties.message,
                        confirmButtonText = modalProperties.confirmButtonText,
                        declineButtonText = modalProperties.declineButtonText,
                        confirmAction = () => StartSession(),
                        declineAction = () => { },
                        startSelection = modalProperties.startSelection
                    });
                });
            }
            else
            {
                startButton.onClick.AddListener(() => StartSession());
            }

            base.Initialize();
        }

        public override void Open()
        {
            base.Open();

            if (RGSKCore.runtimeData.SelectedSession != null)
            {
                RGSKCore.runtimeData.SelectedSession = null;
            }

            RGSKCore.runtimeData.SelectedSession = ScriptableObject.CreateInstance<RaceSession>();
            RGSKCore.runtimeData.SelectedSession.startMode = RaceStartMode.StandingStart;
            RGSKCore.runtimeData.SelectedSession.saveRecords = false;

            foreach (var e in _entries)
            {
                e.UpdateSession(RGSKCore.runtimeData.SelectedSession);
            }

            RefreshEntries();
            StartCoroutine(scrollView?.SelectChild(0));
        }

        public void RefreshEntries()
        {
            foreach (var e in _entries)
            {
                e.SelectOption(0);
                e.ToggleActive();
            }
        }

        void StartSession()
        {
            if (RGSKCore.runtimeData.SelectedTrack == null || RGSKCore.runtimeData.SelectedVehicle == null)
                return;

            RGSKCore.runtimeData.SelectedSession.entrants.Clear();

            RGSKCore.runtimeData.SelectedSession.entrants.Add(new RaceEntrant
            {
                prefab = RGSKCore.runtimeData.SelectedVehicle.prefab,
                colorSelectMode = RGSKCore.runtimeData.SelectedVehicleLivery == null ? ColorSelectionMode.Color : ColorSelectionMode.Livery,
                color = RGSKCore.runtimeData.SelectedVehicleColor,
                livery = RGSKCore.runtimeData.SelectedVehicleLivery,
                isPlayer = true
            });

            RGSKCore.runtimeData.SelectedSession.opponentClass = OpponentClassOptions.SameAsPlayer;
            RGSKCore.runtimeData.SelectedSession.opponentVehicleClass = RGSKCore.runtimeData.SelectedVehicle.vehicleClass;
            RGSKCore.runtimeData.SelectedSession.autoPopulateOpponents = true;

            CalculateRewards();
            SceneLoadManager.LoadScene(RGSKCore.runtimeData.SelectedTrack.scene);
        }

        void CalculateRewards()
        {
            if (!autoCalculateRewards || RGSKCore.runtimeData.SelectedSession.opponentCount == 0)
                return;

            var baseCurrency = baseCurrencyReward * RGSKCore.runtimeData.SelectedSession.opponentCount * RGSKCore.runtimeData.SelectedSession.lapCount;
            var baseXp = baseXpReward * RGSKCore.runtimeData.SelectedSession.opponentCount * RGSKCore.runtimeData.SelectedSession.lapCount;
            var rewardMultiplier = 1f;

            for (int i = 0; i < 3; i++)
            {
                RGSKCore.runtimeData.SelectedSession.raceRewards.Add(new RaceReward
                {
                    currency = (int)(baseCurrency * rewardMultiplier),
                    xp = (int)(baseXp * rewardMultiplier)
                });

                rewardMultiplier -= 0.25f;
            }
        }
    }
}