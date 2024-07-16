using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class CameraPerspectiveTarget : MonoBehaviour
    {
        public List<CameraPerspective> perspectives = new List<CameraPerspective>();
        public Vector3 offset;
    }
}