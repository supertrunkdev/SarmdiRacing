using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomPropertyDrawer(typeof(RaceEntrant))]
    public class RaceEntrantDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            var spacing = 10f;
            var variableWidth = (position.width - spacing) / 3;
            EditorGUIUtility.labelWidth = 60;

            var prefabRect = new Rect(position.x, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var profileRect = new Rect(prefabRect.xMax + spacing, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var playerRect = new Rect(profileRect.xMax + spacing, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var colorModeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);
            var colorRect = new Rect(colorModeRect.xMax + spacing, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);

            var prefabProp = property.FindPropertyRelative("prefab");
            var profileProp = property.FindPropertyRelative("profile");
            var playerProp = property.FindPropertyRelative("isPlayer");
            var colorModeProp = property.FindPropertyRelative("colorSelectMode");
            var colorProp = property.FindPropertyRelative("color");
            var liveryProp = property.FindPropertyRelative("livery");

            EditorGUI.PropertyField(prefabRect, prefabProp);
            EditorGUI.PropertyField(profileRect, profileProp);
            EditorGUI.PropertyField(playerRect, playerProp);
            EditorGUI.PropertyField(colorModeRect, colorModeProp, new GUIContent("Color"));

            if (colorModeProp.enumValueIndex != 0)
            {
                EditorGUI.PropertyField(colorRect, colorModeProp.enumValueIndex == 1 ? colorProp : liveryProp, new GUIContent(""));
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2.2f;
        }
    }
}