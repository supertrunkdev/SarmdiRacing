using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class PreRaceScreen : RaceScreenBase
    {
        [SerializeField] Button startRaceButton;

        public override void Initialize()
        {
            base.Initialize();
            startRaceButton?.onClick?.AddListener(() => RaceManager.Instance?.StartRace());
        }
        
        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnRaceRestart() { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}