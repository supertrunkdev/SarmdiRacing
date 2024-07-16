using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RGSK
{
    public class RaceCountdownSequence : MonoBehaviour
    {
        [SerializeField] PlayableDirector sequence;

        void OnEnable()
        {
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        void Awake() 
        {
            if(RaceManager.Instance != null)
            {
                OnRaceStateChanged(RaceManager.Instance.CurrentState);
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            if (sequence == null)
                return;

            switch (state)
            {
                case RaceState.PreRace:
                default:
                    {
                        sequence.gameObject.SetActive(false);
                        sequence.Stop();
                        break;
                    }

                case RaceState.Countdown:
                    {
                        sequence.gameObject.SetActive(true);
                        sequence.Play();
                        break;
                    }

                case RaceState.Racing:
                    {
                        break;
                    }
            }
        }
    }
}