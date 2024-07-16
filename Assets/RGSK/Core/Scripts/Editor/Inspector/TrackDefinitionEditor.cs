using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(TrackDefinition))]
    public class TrackDefinitionEditor : UnityEditor.Editor
    {
        TrackDefinition _target;

        void OnEnable()
        {
            _target = (TrackDefinition)target;
        }

        public override void OnInspectorGUI()
        {
            EditorHelper.DrawAddToListUI(RGSKCore.Instance.ContentSettings.tracks, _target);

            using (new GUILayout.VerticalScope("Box"))
            {
                DrawDefaultInspector();
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"Lap Record: {UIHelper.FormatTimeText(_target.LoadBestLap())}");
                }
            }
        }
    }
}