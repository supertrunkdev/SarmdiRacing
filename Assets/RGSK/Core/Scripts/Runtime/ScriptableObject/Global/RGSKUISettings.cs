using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/UI")]
    public class RGSKUISettings : ScriptableObject
    {
        [Header("Formats")]
        public TimeFormat raceTimerFormat = TimeFormat.MM_SS_FFF;
        public TimeFormat realtimeGapTimerFormat = TimeFormat.S_FFF;
        public SpeedUnit speedUnit;
        public DistanceUnit distanceUnit;

        [Header("Audio")]
        public string buttonHoverSound = "button_hover";
        public string buttonClickSound = "button_click";

        [Header("Screens")]
        public UIScreenID preRaceScreen;
        public UIScreenID raceScreen;
        public UIScreenID postRaceScreen;
        public UIScreenID pauseScreen;
        public UIScreenID replayScreeen;

        [Header("Modal Windows")]
        public List<ModalWindow> modalWindowPrefabs = new List<ModalWindow>();

        [Header("Race Type UI")]
        public RaceUILayout defaultRaceUILayout;
        public List<RaceUILayout> raceUILayouts = new List<RaceUILayout>();

        [Header("Race Boards")]
        public RaceBoardLayout defaultRaceBoardLayout;
        public List<RaceBoardLayout> raceBoardLayouts = new List<RaceBoardLayout>();
        public NumberDisplayMode raceBoardPositionFormat = NumberDisplayMode.Single;
        public NumberDisplayMode raceBoardlapFormat = NumberDisplayMode.Single;
        public VehicleNameDisplayMode raceBoardVehicleNameFormat = VehicleNameDisplayMode.FullName;
        public NameDisplayMode raceBoardNameFormat = NameDisplayMode.FullName;

        [Header("Worldspace")]
        public Nameplate nameplate;
        public RaceWaypointArrow waypointArrow;
        public bool showNameplates = true;
        public bool showWaypointArrow;

        [Header("Minimap")]
        public string playerMinimapBlip = "player";
        public string opponentMinimapBlip = "opponent";

        [Header("Misc")]
        public string dnfString = "DNF";
        public string currencyFormat = "Cr {0}";
        public List<TargetScoreIcon> targetScoreIcons = new List<TargetScoreIcon>();
    }
}