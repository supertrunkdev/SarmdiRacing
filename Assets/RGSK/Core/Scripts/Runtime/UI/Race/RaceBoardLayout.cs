using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace RGSK
{
    [System.Serializable]
    public class BoardItem
    {
        public string name;

        [Header("Cell")]
        public BoardCellType cellType;
        public RaceBoardCellValue cellValue;

        [Header("Header")]
        public string header;

        [Header("Text")]
        public TMP_FontAsset font;
        public float fontSize = 0;
        public TextAlignmentOptions alignment;

        [Header("Layout Element")]
        public float preferredWidth = 100;
        public float preferredHeight = 50;
        public float flexibleWidth = 0;
        public float flexibleHeight = 0;
    }

    public enum BoardCellType
    {
        Text,
        Image
    }

    public enum RaceBoardCellValue
    {
        [InspectorName("Profile/Name")] ProfileName,
        [InspectorName("Profile/Country")] ProfileCountry,

        [InspectorName("Vehicle/Name")] VehicleName,
        [InspectorName("Vehicle/Manufacturer Logo")] VehicleManufacturerLogo,

        [InspectorName("Competitor/Position")] Position,
        [InspectorName("Competitor/Lap")] CurrentLap,
        [InspectorName("Competitor/Best Lap")] BestLapTime,
        [InspectorName("Competitor/Race Percentage")] RacePercentage,
        [InspectorName("Competitor/Distance")] Distance,
        [InspectorName("Competitor/Checkpoints")] Checkpoints,
        [InspectorName("Competitor/Total Speed")] TotalSpeed,
        [InspectorName("Competitor/Total Laps")] TotalLaps,
        [InspectorName("Competitor/Finish Time")] FinishTime,
        [InspectorName("Competitor/Finish Gap")] FinishGap,
        [InspectorName("Competitor/Overtakes")] Overtakes,
        [InspectorName("Competitor/Score")] Score,
        [InspectorName("Competitor/AverageSpeed")] AverageSpeed,
        [InspectorName("Competitor/Leader Distance Gap")] DistanceGap,
        [InspectorName("Competitor/Leader Time Gap")] TimeGap,

        [InspectorName("Drift/Total Points")] TotalDriftPoints,

        [InspectorName("Profile/Avatar")] ProfileAvatar,

        [InspectorName("Competitor/Top Speed")] TopSpeed,
        [InspectorName("Competitor/Best Lap Gap")] BestLapGap,
    };

    [CreateAssetMenu(menuName = "RGSK/UI/Race Board Layout")]
    public class RaceBoardLayout : ScriptableObject
    {
        public string title;
        public List<RaceType> raceTypes = new List<RaceType>();
        public List<BoardItem> items = new List<BoardItem>();
        public List<BoardItem> miniItems = new List<BoardItem>();
    }
}