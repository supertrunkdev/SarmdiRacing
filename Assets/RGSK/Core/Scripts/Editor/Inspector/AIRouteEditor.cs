using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK.Editor
{
    [CustomEditor(typeof(AIRoute))]
    public class AIRouteEditor : RouteEditor
    {
        AIRoute _aiRoute;

        SerializedProperty speedZones;
        SerializedProperty speedLimit;
        SerializedProperty priority;
        SerializedProperty branchProbability;
        SerializedProperty showRacingLineGizmos;
        SerializedProperty showSpeedZoneGizmos;

        protected override void OnEnable()
        {
            base.OnEnable();

            _aiRoute = (AIRoute)_target;
            SortSpeedzones();

            speedZones = serializedObject.FindProperty(nameof(speedZones));
            speedLimit = serializedObject.FindProperty(nameof(speedLimit));
            priority = serializedObject.FindProperty(nameof(priority));
            branchProbability = serializedObject.FindProperty(nameof(branchProbability));
            showRacingLineGizmos = serializedObject.FindProperty(nameof(showRacingLineGizmos));
            showSpeedZoneGizmos = serializedObject.FindProperty(nameof(showSpeedZoneGizmos));
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (_aiRoute.showSpeedZoneGizmos)
            {
                DrawSpeedZones();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("AI", EditorStyles.boldLabel);

                EditorGUILayout.HelpBox("With the tool selected in the scene view, use CTRL + Left Mouse Button to draw the racing line or insert speedzones.", MessageType.Info);
                EditorGUILayout.PropertyField(priority);
                EditorGUILayout.PropertyField(speedLimit);
                EditorGUILayout.PropertyField(branchProbability);

                EditorHelper.DrawLine();

                EditorGUILayout.PropertyField(showRacingLineGizmos, new GUIContent("Show Racing Line"));
                EditorGUILayout.PropertyField(showSpeedZoneGizmos, new GUIContent("Show Speedzones")); 

                EditorHelper.DrawLine();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(speedZones);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            base.OnHit(position, normal);
            _aiRoute.UpdateNodeRotation();
        }

        protected override void OnRouteClicked(float normalizedDistance, RaycastHit hit)
        {
            switch (_aiRoute.insertMode)
            {
                case AIRouteInsertMode.Default:
                    {
                        base.OnRouteClicked(normalizedDistance, hit);
                        _aiRoute.UpdateNodeRotation();
                        break;
                    }

                case AIRouteInsertMode.RacingLine:
                    {
                        var closest = (AINode)_aiRoute.GetClosestNode(hit.point);
                        var hitVector = hit.point - closest.transform.position;

                        Undo.RecordObject(closest, "inserted_racingline");
                        closest.RacingLineOffset = Vector3.Dot(hitVector, closest.transform.right);
                        break;
                    }

                case AIRouteInsertMode.Speedzones:
                    {
                        normalizedDistance *= _aiRoute.Distance;

                        var speedzone = new AISpeedZone
                        {
                            startDistance = normalizedDistance,
                            endDistance = normalizedDistance + 100,
                            speedLimit = -1
                        };

                        Undo.RecordObject(_aiRoute, "inserted_speedzone");
                        _aiRoute.speedZones.Add(speedzone);
                        SortSpeedzones();
                        break;
                    }
            }
        }

        protected override void DrawNodeHandles()
        {
            Handles.matrix = _target.transform.localToWorldMatrix;

            for (int i = 0; i < _target.nodes.Count; ++i)
            {
                var node = (AINode)_target.nodes[i];
                var pos = node.transform.position + (node.transform.right * node.RacingLineOffset);
                var percent = $"\n{UIHelper.FormatDistanceText(node.normalizedDistance * _target.Distance)}({(node.normalizedDistance * 100).ToString("F1")}%)"; var branches = "";

                if (node.branchRoutes.Count > 0)
                {
                    branches = $"\n[Branches: {node.branchRoutes.Count}]";
                }

                DrawNodeSelectionButton(node, pos);
                EditorHelper.DrawLabelWithinDistance(pos + Vector3.up * 3, $"{i.ToString()}{percent}{branches}", CustomEditorStyles.nodeLabel);
            }
        }

        void DrawSpeedZones()
        {
            if (_aiRoute.speedZones.Count == 0)
                return;

            Handles.color = RGSKEditorSettings.Instance.aiSpeedzoneColor;

            for (int i = 0; i < _aiRoute.speedZones.Count; i++)
            {
                var startDistance = _aiRoute.speedZones[i].startDistance;
                var endDistance = _aiRoute.speedZones[i].endDistance;
                var factor = _aiRoute.speedZones[i].speedLimit;
                var boost = _aiRoute.speedZones[i].boostProbability;

                if (_aiRoute.Distance < startDistance)
                {
                    continue;
                }

                var next = (endDistance - startDistance) / 20;
                for (float j = startDistance; j < endDistance; j += next)
                {
                    Vector3 start = _target.GetPositionAtDistance(j);
                    Vector3 end = _target.GetPositionAtDistance(j + next);

                    start.y += 2;
                    end.y += 2;

                    Handles.DrawLine(start, end);
                }

                var index = i + 1;
                var speed = factor > -1 ? UIHelper.FormatSpeedText(factor, true) : $"Unlimited";
                var labelPos = _target.GetPositionAtDistance(startDistance);
                labelPos.y += 2;

                EditorHelper.DrawLabelWithinDistance(labelPos,
                        $"[{index.ToString()}]\nMax Speed: {speed}\nBoost Probability: {boost}",
                        CustomEditorStyles.sceneViewBox);
            }
        }

        void SortSpeedzones()
        {
            _aiRoute.speedZones = _aiRoute.speedZones.OrderBy(x => x.startDistance).ToList();
        }

        public override RouteNode CreateNode(GameObject node = null)
        {
            var n = node != null ? node.GetOrAddComponent<AINode>() : new GameObject("AINode").AddComponent<AINode>();
            return n;
        }
    }
}