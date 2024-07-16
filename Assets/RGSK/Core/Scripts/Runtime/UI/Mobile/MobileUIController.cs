using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class MobileUIController : MonoBehaviour
    {
        [SerializeField] GameObject touchSteer;
        [SerializeField] GameObject wheelSteer;
        [SerializeField] GameObject tiltSteer;
        [SerializeField] GameObject manualButtons;
        [SerializeField] GameObject nitrousButton;

        void FixedUpdate()
        {
            if (touchSteer != null)
            {
                touchSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Touch);
            }

            if (wheelSteer != null)
            {
                wheelSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Wheel);
            }

            if (tiltSteer != null)
            {
                tiltSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Tilt);
            }

            if (manualButtons != null)
            {
                manualButtons.SetActiveSafe(RGSKCore.Instance.VehicleSettings.transmissionType == TransmissionType.Manual);
            }

            if (nitrousButton != null && PlayerVehicleInput.Instance?.Vehicle != null)
            {
                nitrousButton.SetActive(PlayerVehicleInput.Instance.Vehicle.HasNitrous);
            }
        }
    }
}