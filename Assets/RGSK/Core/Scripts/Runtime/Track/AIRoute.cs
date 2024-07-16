using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using UnityEngine.Serialization;

namespace RGSK
{
    [System.Serializable]
    public class AISpeedZone
    {
        public float startDistance;
        public float endDistance;
        public float speedLimit = -1;
        [Range(0, 1)] public float boostProbability = 0f;
    }

    public enum AIRouteInsertMode
    {
        Default,
        RacingLine,
        Speedzones
    }

    public class AIRoute : Route
    {
        public List<AISpeedZone> speedZones = new List<AISpeedZone>();
        public float speedLimit = -1;
        public float priority = 10;
        [Range(0, 1)] public float branchProbability = 0.5f;
        public AIRouteInsertMode insertMode;
        public bool showRacingLineGizmos = true;
        public bool showSpeedZoneGizmos = true;

        public AISpeedZone GetSpeedZone(float distance)
        {
            foreach (var zone in speedZones)
            {
                if (distance.IsBetween(zone.startDistance, zone.endDistance))
                {
                    return zone;
                }
            }

            return null;
        }

        public override void Reverse()
        {
            base.Reverse();

            foreach (var node in nodes)
            {
                var aiNode = (AINode)node;
                aiNode.RacingLineOffset *= -1;
            }
        }

        protected override void DrawRoute()
        {
            if (showRacingLineGizmos)
            {
                DrawRacingLine();
            }

            if (showRoute)
            {
                DrawWidth();
            }
        }

        void DrawRacingLine()
        {
            Gizmos.color = RGSKEditorSettings.Instance.racingLineColor;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (i < nodes.Count - 1)
                {
                    var current = (AINode)nodes[i];
                    var next = (AINode)nodes[i + 1];

                    Gizmos.DrawLine(current.transform.position + (current.transform.right * current.RacingLineOffset),
                                    next.transform.position + (next.transform.right * next.RacingLineOffset));
                }
            }

            if (loop && nodes.Count > 2)
            {
                var last = (AINode)nodes[nodes.Count - 1];
                var first = (AINode)nodes[0];

                Gizmos.DrawLine(last.transform.position + (last.transform.right * last.RacingLineOffset),
                                first.transform.position + (first.transform.right * first.RacingLineOffset));
            }
        }

        void DrawWidth()
        {
            Gizmos.color = gizmoColor;

            if (nodes.Count < 1)
                return;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Gizmos.DrawLine(nodes[i].transform.position + (nodes[i].transform.right * nodes[i].rightWidth),
                    nodes[i + 1].transform.position + (nodes[i + 1].transform.right * nodes[i + 1].rightWidth));

                Gizmos.DrawLine(nodes[i].transform.position + (-nodes[i].transform.right * nodes[i].leftWidth),
                   nodes[i + 1].transform.position + (-nodes[i + 1].transform.right * nodes[i + 1].leftWidth));
            }

            if (loop)
            {
                Gizmos.DrawLine(nodes[nodes.Count - 1].transform.position + (nodes[nodes.Count - 1].transform.right * nodes[nodes.Count - 1].rightWidth),
                nodes[0].transform.position + (nodes[0].transform.right * nodes[0].rightWidth));

                Gizmos.DrawLine(nodes[nodes.Count - 1].transform.position + (-nodes[nodes.Count - 1].transform.right * nodes[nodes.Count - 1].leftWidth),
                nodes[0].transform.position + (-nodes[0].transform.right * nodes[0].leftWidth));
            }
        }
    }
}