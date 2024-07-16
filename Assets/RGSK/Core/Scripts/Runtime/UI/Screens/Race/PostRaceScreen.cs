using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace RGSK
{
    public class PostRaceScreen : RaceScreenBase
    {
        [SerializeField] PlayableDirector finishSequence;
        [SerializeField] UIScreenID rewardsScreenID;
        [SerializeField] Button continueButton;

        bool _skipFinishSequence;

        public override void Initialize()
        {
            base.Initialize();
            continueButton?.onClick?.AddListener(Continue);
            _skipFinishSequence = false;
        }

        public override void Open()
        {
            base.Open();

            if(finishSequence != null)
            {   
                if(!_skipFinishSequence)
                {
                    _skipFinishSequence = true;
                    finishSequence.initialTime = 0;
                    finishSequence.Play();
                }
                else
                {
                    finishSequence.initialTime = finishSequence.duration;
                    finishSequence.Play();
                }
            }
        }

        void Continue()
        {
            //If there are rewards, open the reward screen, else skip it
            if (RaceRewardManager.Instance.Reward != null)
            {
                rewardsScreenID?.Open();
            }
            else
            {
                SceneLoadManager.LoadMainScene();
            }
        }

        protected override void OnRaceRestart() 
        { 
            _skipFinishSequence = false;
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}