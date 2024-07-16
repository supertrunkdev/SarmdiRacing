using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RGSK.Helpers;
using RGSK.Extensions;
using TMPro;

namespace RGSK
{
    public class RaceRewardsScreen : UIScreen
    {
        [SerializeField] Button continueButton;
        [SerializeField] TMP_Text currencyText;
        [SerializeField] TMP_Text xpText;
        [SerializeField] XPGauge xpGauge;

        public override void Initialize()
        {
            base.Initialize();
            continueButton?.onClick.AddListener(Continue);
        }

        public override void Open()
        {
            base.Open();

            var totalXP = SaveData.Instance.playerData.xp;

            currencyText?.SetText($"+ {UIHelper.FormatCurrencyText(RaceRewardManager.Instance.Reward.currency)}");
            xpText?.SetText($"+ {UIHelper.FormatPointsText(RaceRewardManager.Instance.Reward.xp)}");

            StopAllCoroutines();
            StartCoroutine(XPGaugeFillRoutine(totalXP - RaceRewardManager.Instance.Reward.xp, totalXP));
        }

        IEnumerator XPGaugeFillRoutine(int oldXP, int newXP)
        {
            var timer = 0f;
            var xp = 0f;

            while (timer < 5)
            {
                timer += Time.deltaTime;
                xp = Mathf.Lerp(oldXP, newXP, timer / 5);
                xpGauge?.SetValue((int)xp);
                yield return null;
            }
        }

        void Continue()
        {
            SceneLoadManager.LoadMainScene();
        }
    }
}