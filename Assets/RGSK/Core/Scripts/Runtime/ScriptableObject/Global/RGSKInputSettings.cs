using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Input")]
    public class RGSKInputSettings : ScriptableObject
    {
        public MobileControlType mobileControlType;
        public bool vibrate = true;
    }
}