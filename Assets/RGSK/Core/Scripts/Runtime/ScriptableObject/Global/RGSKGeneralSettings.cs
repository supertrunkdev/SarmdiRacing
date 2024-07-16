using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/General")]
    public class RGSKGeneralSettings : ScriptableObject
    {
        [Header("Persistent")]
        public GameObject[] persistentObjects;

        [Header("Runtime Sets")]
        public EntityRuntimeSet entitySet;

        [Header("Settings")]
        public CameraSettings cameraSettings;
        public DriftSettings driftSettings;
        public SlipstreamSettings slipstreamSettings;
        public RepositionSettings repositionSettings;
        public MinimapSettings minimapSettings;
        public RecorderSettings recorderSettings;
        public CountrySettings countrySettings;

        [Header("Player")]
        public ProfileDefinition playerProfile;
        public XPCurve playerXPCurve;

        [Header("Save")]
        public string saveFileName = "profile1";
        public bool enableSaveSystem = true;
        public bool encryptSaveData = true;
        public bool saveOnApplicationQuit = true;

        [Header("Layers")]
        public LayerMaskSingle vehicleLayerIndex;
        public LayerMaskSingle ghostLayerIndex;
        public LayerMaskSingle minimapLayerIndex;

        [Header("Util")]
        public GameObject terminal;
        public bool includeTerminalInBuild;
        public bool enableFpsReader = true;
        public bool enableLogs = true;
    }
}