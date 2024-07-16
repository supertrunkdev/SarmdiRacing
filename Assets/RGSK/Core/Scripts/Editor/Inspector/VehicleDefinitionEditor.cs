using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(VehicleDefinition))]
    public class VehicleDefinitionEditor : UnityEditor.Editor
    {
        VehicleDefinition _target;

        void OnEnable()
        {
            _target = (VehicleDefinition)target;
        }

        public override void OnInspectorGUI()
        {
            EditorHelper.DrawAddToListUI(RGSKCore.Instance.ContentSettings.vehicles, _target);

            using (new GUILayout.VerticalScope("Box"))
            {
                DrawDefaultInspector();
            }
        }
    }
}