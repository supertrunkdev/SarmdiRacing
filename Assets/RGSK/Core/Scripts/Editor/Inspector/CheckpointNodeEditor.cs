using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CheckpointNode))]
    [CanEditMultipleObjects]
    public class CheckpointNodeEditor : RouteNodeEditor
    {
        CheckpointNode _target2;
        SerializedProperty type;
        SerializedProperty timeExtend;

        protected override void OnEnable()
        {
            base.OnEnable();

            _target2 = (CheckpointNode)target;

            type = serializedObject.FindProperty(nameof(type));
            timeExtend = serializedObject.FindProperty(nameof(timeExtend));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Checkpoint Type", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(type);

                if (_target2.IsTimeExtend)
                {
                    EditorGUILayout.PropertyField(timeExtend);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}