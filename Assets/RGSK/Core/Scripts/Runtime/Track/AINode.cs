using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class AINode : RouteNode
    {
        [Range(-1, 1)][SerializeField] float racingLineOffset = 0;
        public List<AIRoute> branchRoutes = new List<AIRoute>();

        public float RacingLineOffset
        {
            get
            {
                return Mathf.Lerp(-leftWidth, rightWidth, (racingLineOffset + 1) / 2);
            }
            set
            {
                racingLineOffset = Mathf.InverseLerp(-leftWidth, rightWidth, value) * 2 - 1;
            }
        }

        void Awake() 
        {
            branchRoutes.RemoveNullElements();
        }

        protected override void DrawGizmos()
        {
            Gizmos.DrawSphere(transform.position + (transform.right * RacingLineOffset), 0.5f);

            if (branchRoutes.Count > 0)
            {
                foreach (var branch in branchRoutes)
                {
                    if (branch != null)
                    {
                        var closest = branch.GetClosestNode(transform.position);

                        if (closest != null)
                        {
                            Gizmos.DrawLine(transform.position, closest.transform.position);
                        }
                    }
                }
            }
        }
    }
}