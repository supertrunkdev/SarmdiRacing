using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] ProfileDefinitionUI profileDefinitionUI;

        [Header("Currency/XP")]
        [SerializeField] TMP_Text currencyText;
        [SerializeField] XPGauge xpGauge;

        [Header("Stats")]
        [SerializeField] TMP_Text totalWinsText;
        [SerializeField] TMP_Text totalRacesText;
        [SerializeField] TMP_Text winRatioText;
        [SerializeField] TMP_Text totalDistanceText;

        void Start()
        {
            if (profileDefinitionUI != null)
            {
                profileDefinitionUI.profile = RGSKCore.Instance.GeneralSettings.playerProfile;
            }

            UpdateUI();
        }

        public void UpdateUI()
        {
            profileDefinitionUI?.UpdateUI();

            currencyText?.SetText(UIHelper.FormatCurrencyText(SaveData.Instance.playerData.currency));
            xpGauge?.SetValue(SaveData.Instance.playerData.xp);

            var totalWins = SaveData.Instance.playerData.totalWins;
            var totalRaces = SaveData.Instance.playerData.totalRaces;

            totalWinsText?.SetText(totalWins.ToString());
            totalRacesText?.SetText(totalRaces.ToString());
            winRatioText?.SetText(UIHelper.FormatPercentageText(((float)totalWins / (float)Mathf.Clamp(totalRaces, 1, totalRaces)) * 100));
            totalDistanceText?.SetText(UIHelper.FormatDistanceText(SaveData.Instance.playerData.totalDistance));
        }
    }
}