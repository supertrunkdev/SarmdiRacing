using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class TransformFollower : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, 0);

        void Update()
        {
            if (target == null)
                return;

            var pos = target.position;
            var rot = transform.eulerAngles;

            pos += offset;
            rot.y = target.eulerAngles.y;

            transform.position = pos;
            transform.eulerAngles = rot;
        }
    }
}