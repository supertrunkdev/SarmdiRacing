using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    [Serializable]
    public class SaveData
    {
        public static SaveData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveData();
                }

                return _instance;
            }
            set
            {
                if (value != null)
                {
                    _instance = value;
                }
            }
        }

        static SaveData _instance;
        public PlayerData playerData = new PlayerData();
        public GameSettingsData gameSettingsData = new GameSettingsData();
        public Dictionary<string, float> bestLaps = new Dictionary<string, float>();
        public Dictionary<string, int> bestSessionPosition = new Dictionary<string, int>();
        public List<string> unlockedItems = new List<string>();
    }

    [Serializable]
    public class PlayerData
    {
        //profile
        public string firstName = "Player";
        public string lastName = "One";
        public int countryIndex = 0;

        //info
        public string selectedVehicleID = "";
        public int currency = 0;
        public int xp = 0;
        public int totalWins = 0;
        public int totalRaces = 0;
        public int totalDistance = 0;
    }

    [Serializable]
    public class GameSettingsData
    {
        //graphics
        public int qualityIndex = QualitySettings.GetQualityLevel();
        public int resolutionIndex = GeneralHelper.GetCurrentResolutionIndex();
        public bool fullscreen = true;
        public int vSyncCount = 0;
        public int targetFPS = -1;

        //audio
        public Dictionary<AudioGroup, float> volumes = new Dictionary<AudioGroup, float>
        {
            {AudioGroup.Master, 1},
            {AudioGroup.Music, 0.7f},
            {AudioGroup.SFX, 0.7f},
        };

        //gameplay
        public int cameraPerspectiveIndex = 0;
        public SpeedUnit speedUnit = RGSKCore.Instance.UISettings.speedUnit;
        public DistanceUnit distanceUnit = RGSKCore.Instance.UISettings.distanceUnit;
        public TransmissionType transmission = TransmissionType.Automatic;
        public MobileControlType mobileInput = RGSKCore.Instance.InputSettings.mobileControlType;
        public bool showNameplates = RGSKCore.Instance.UISettings.showNameplates;
        public bool vibrate = RGSKCore.Instance.InputSettings.vibrate;
        public bool showFPS = RGSKCore.Instance.GeneralSettings.enableFpsReader;
    }
}