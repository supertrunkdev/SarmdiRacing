using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Content")]
    public class RGSKContentSettings : ScriptableObject
    {
        public List<TrackDefinition> tracks = new List<TrackDefinition>();
        public List<VehicleDefinition> vehicles = new List<VehicleDefinition>();

        void OnEnable()
        {
            tracks.RemoveNullElements();
            vehicles.RemoveNullElements();
        }
    }
}