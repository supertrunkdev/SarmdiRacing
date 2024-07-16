using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class VehicleStats
    {
        [Header("Performance")]
        [Range(0, 1)] public float speed;
        [Range(0, 1)] public float acceleration;
        [Range(0, 1)] public float braking;
        [Range(0, 1)] public float handling;

        [Header("Color")]
        public int color;
    }

    [CreateAssetMenu(menuName = "RGSK/Vehicle/Vehicle Definition")]
    public class VehicleDefinition : ItemDefinition
    {
        [Space(10)]
        public GameObject prefab;
        public VehicleManufacturer manufacturer;
        public VehicleClass vehicleClass;

        [Header("Stats")]
        public VehicleStats defaultStats;

        [Header("Extra Details")]
        public string year;
    }
}