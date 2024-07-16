using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] Gauge progressBar;
        [SerializeField] TMP_Text progressText;

        void Update()
        {
            SetBarValue(SceneLoadManager.LoadingProgress);
        }

        public void SetBarValue(float value)
        {
            progressBar?.SetValue(value);

            if (progressText != null)
            {
                progressText.text = UIHelper.FormatPercentageText(value * 100);
            }
        }
    }
}