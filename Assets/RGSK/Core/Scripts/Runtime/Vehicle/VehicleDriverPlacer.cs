using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class VehicleDriverPlacer : MonoBehaviour
    {
        VehicleDriver _driver;

        void Start()
        {
            var seat = GetComponentInChildren<VehicleSeat>();
            var driver = GetComponentInChildren<VehicleDriver>();

            if (seat == null)
            {
                Logger.Log(this, "A driver cannot be created because this vehicle has no seats!");
                return;
            }

            if(driver == null)
            {
                var randomDriver = RGSKCore.Instance.VehicleSettings.vehicleDrivers.GetRandom();
                
                if (randomDriver != null)
                {
                    _driver = Instantiate(randomDriver);
                    _driver.PlaceInVehicle(gameObject);
                }
            }
        }

        public void ToggleDriverVisibility(bool visible)
        {
            if (_driver != null)
            {
                _driver.gameObject.SetActive(visible);
            }
        }
    }
}