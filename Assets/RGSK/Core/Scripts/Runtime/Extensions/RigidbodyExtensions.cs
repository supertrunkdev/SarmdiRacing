using UnityEngine;

namespace RGSK.Extensions
{
    public static class RigidbodyExtensions
    {
        /// <summary>
        /// Returns the speed in meters per second.
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static float SpeedInMPS(this Rigidbody rb)
        {
            return rb.velocity.magnitude;
        }

        /// <summary>
        /// Returns the speed in kilometers per hour.
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static float SpeedInKPH(this Rigidbody rb)
        {
            return rb.velocity.magnitude * 3.6f;
        }

        /// <summary>
        /// Returns the speed in miles per hour.
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static float SpeedInMPH(this Rigidbody rb)
        {
            return rb.velocity.magnitude * 2.237f;
        }

        /// <summary>
        /// Sets the speed of the rigidbody instantaneously.
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="speed"></param>
        /// <param name="unit"></param>
        public static void SetSpeed(this Rigidbody rb, float speed, SpeedUnit unit)
        {
            switch (unit)
            {
                case SpeedUnit.KMH:
                    {
                        speed = speed / 3.6f;
                        break;
                    }

                case SpeedUnit.MPH:
                    {
                        speed = speed / 2.237f;
                        break;
                    }
            }

            rb.velocity = rb.transform.forward * speed;
        }

        /// <summary>
        /// Resets all of the rigidbody's velocity.
        /// </summary>
        /// <param name="rb"></param>
        public static void ResetVelocity(this Rigidbody rb)
        {
            if(rb == null)
                return;
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Returns 1 if direction of travel forwards and -1 if backwards.
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static float TravelDirection(this Rigidbody rb)
        {
            float dir = rb.transform.InverseTransformDirection(rb.velocity).z;

            if (dir > 0) return 1;
            if (dir < 0) return -1;

            return 0;
        }
    }
}