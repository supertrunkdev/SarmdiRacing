using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Race")]
    public class RGSKRaceSettings : ScriptableObject
    {
        public List<RaceType> raceTypes = new List<RaceType>();

        [Header("Checkpoints")]
        public CheckpointGate checkpointGate;
        public bool showCheckpointGates;

        [Header("Audio")]
        public SoundList preRaceMusic;
        public SoundList raceMusic;
        public SoundList postRaceMusic;
        public string checkpointHitSound = "checkpoint";
        public string raceFinishSound = "race_finish";
        public string raceDisqualifySound = "race_dq";

        [Header("DNF")]
        [Tooltip("How the DNF timer start should be handled." +
                 "\n\nAfter First Finish - start the timer after 1 competitor finishes the race." +
                 "\n\nAfter Half Finish - start the timer after half the competitors finish the race.")]
        public DnfTimerStartMode dnfTimerStartMode;

        [Tooltip("The value that the DNF timer starts at.")]
        public float dnfTimeLimit = 60;

        [Header("Replay")]
        public bool enableReplay = true;
        
        [Header("Misc")]
        [Tooltip("The race states that the route cameras should activate in.")]
        [EnumFlags] public RaceState cinematicCameraStates;

        public float rollingStartCountdownWait = 5;
        public bool ghostDisqualifiedCompetitors = true;
        public bool skipPreRaceState;
        public bool wrongwayTracking = true;
    }
}