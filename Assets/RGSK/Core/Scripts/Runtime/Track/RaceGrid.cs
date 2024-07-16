using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class RaceGrid : MonoBehaviour
    {
        public RaceStartMode gridType;

        public List<Transform> GetPositions()
        {
            var children = GetComponentsInChildren<Transform>().ToList();
            children.RemoveAt(0);

            return children;
        }
    }
}