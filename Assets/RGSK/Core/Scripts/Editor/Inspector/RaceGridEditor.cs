using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceGrid))]
    public class RaceGridEditor : SceneViewRaycaster
    {
        RaceGrid _target;

        SerializedProperty gridType;

        GizmoDrawer[] _gizmoDrawers;

        void OnEnable()
        {
            _target = (RaceGrid)target;
            gridType = serializedObject.FindProperty(nameof(gridType));
            _gizmoDrawers = _target.GetComponentsInChildren<GizmoDrawer>();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use Shift + Left Mouse Button to place a grid position\n\nUse Shift + Left Mouse while hovering over a grid position to delete it.", MessageType.Info);
            EditorHelper.DrawLine();
            EditorGUILayout.PropertyField(gridType);

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnSceneGUI()
        {
            var e = Event.current;

            for (int i = 0; i < _target.transform.childCount; ++i)
            {
                var node = _target.transform.GetChild(i);
                var pos = node.transform.position;

                EditorHelper.DrawLabelWithinDistance(pos + Vector3.up * 3, $"P{(i + 1).ToString()}", CustomEditorStyles.nodeLabel);

                if (Handles.Button(pos, Quaternion.identity, 1, 1, Handles.SphereHandleCap))
                {
                    if (e.shift)
                    {
                        Undo.RecordObject(_target, "deleted_node");
                        Undo.DestroyObjectImmediate(node.gameObject);
                        e.Use();
                    }
                    else
                    {
                        Selection.activeObject = node.gameObject;
                    }
                }
            }

            if (_gizmoDrawers.Length > 0)
            {
                foreach (var g in _gizmoDrawers)
                {
                    if (g != null)
                    {
                        g.gizmoColor = _target.gridType == RaceStartMode.StandingStart ?
                                        RGSKEditorSettings.Instance.standingStartGridColor :
                                        RGSKEditorSettings.Instance.rollingStartGridColor;
                    }
                }
            }

            base.OnSceneGUI();
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            var go = new GameObject();
            go.transform.position = position + new Vector3(0, 0.1f, 0);
            go.transform.SetParent(_target.transform);
            go.name = $"GridPosition";

            var gizmo = go.AddComponent<GizmoDrawer>();
            gizmo.gizmoShape = GizmoDrawer.GizmoShape.Cube;
            gizmo.visualizeDirection = true;
            gizmo.transform.localScale = new Vector3(2, 0.5f, 4);

            Undo.RegisterCreatedObjectUndo(go, "created_grid_position");
            Undo.RecordObject(_target, "added_grid_position");

            _gizmoDrawers = _target.GetComponentsInChildren<GizmoDrawer>();
        }
    }
}