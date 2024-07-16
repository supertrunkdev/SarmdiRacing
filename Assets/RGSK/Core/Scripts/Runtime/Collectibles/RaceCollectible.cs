using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public abstract class RaceCollectible : MonoBehaviour
    {
        [SerializeField] GameObject visual;
        [SerializeField] string collectSound;

        bool _collected;

        void OnEnable()
        {
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            OnRaceRestart();
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        void OnTriggerEnter(Collider other)
        {
            if (_collected)
                return;

            var entity = other.GetComponentInParent<RGSKEntity>();

            if (entity != null && entity.Competitor != null)
            {
                if (entity.Competitor.IsRacing())
                {
                    Collect(entity);
                    ToggleVisual(false);
                    AudioManager.Instance?.Play(collectSound, AudioGroup.SFX);
                    _collected = true;
                }
            }
        }

        protected abstract void Collect(RGSKEntity entity);

        void OnRaceRestart()
        {
            _collected = false;
            ToggleVisual(true);
        }

        void ToggleVisual(bool toggle)
        {
            if (visual != null)
            {
                visual.SetActive(toggle);
            }
        }
    }
}