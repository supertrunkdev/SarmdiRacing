using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    public static class CustomEditorStyles
    {
        public static GUIStyle nodeLabel;
        public static GUIStyle titleLabel;
        public static GUIStyle verticalToolbarButton;
        public static GUIStyle horizontalToolbarButton;
        public static GUIStyle menuLabelCenter;
        public static GUIStyle menuLabelLeft;
        public static GUIStyle menuDescription;
        public static GUIStyle sceneViewBox;

        public static GUIContent greenIconContent;
        public static GUIContent redIconContent;
        public static GUIContent yellowIconContent;
        public static GUIContent nullIconContent;
        public static GUIContent menuIconContent;

        static CustomEditorStyles()
        {
            if (menuLabelCenter == null)
            {
                menuLabelCenter = new GUIStyle(EditorStyles.largeLabel);
                menuLabelCenter.richText = true;
                menuLabelCenter.wordWrap = true;
                menuLabelCenter.alignment = TextAnchor.MiddleCenter;
                menuLabelCenter.fontSize = 12;
            }

            if (menuLabelLeft == null)
            {
                menuLabelLeft = new GUIStyle(EditorStyles.largeLabel);
                menuLabelLeft.richText = true;
                menuLabelLeft.wordWrap = false;
                menuLabelLeft.alignment = TextAnchor.MiddleLeft;
                menuLabelLeft.fontSize = 12;
            }

            if (nodeLabel == null)
            {
                nodeLabel = new GUIStyle(EditorStyles.boldLabel);
                nodeLabel.richText = true;
                nodeLabel.alignment = TextAnchor.MiddleCenter;
            }

            if (titleLabel == null)
            {
                titleLabel = new GUIStyle(EditorStyles.boldLabel);
                titleLabel.alignment = TextAnchor.MiddleLeft;
                titleLabel.fontSize = 20;
            }

            if (verticalToolbarButton == null)
            {
                verticalToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
                verticalToolbarButton.alignment = TextAnchor.MiddleLeft;
            }

            if (horizontalToolbarButton == null)
            {
                horizontalToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
                horizontalToolbarButton.alignment = TextAnchor.MiddleCenter;
            }

            if (menuDescription == null)
            {
                menuDescription = new GUIStyle(EditorStyles.label);
                menuDescription.wordWrap = true;
                menuDescription.alignment = TextAnchor.UpperLeft;
            }

            if (sceneViewBox == null)
            {
                sceneViewBox = new GUIStyle(EditorStyles.boldLabel);
                sceneViewBox.richText = true;
                sceneViewBox.alignment = TextAnchor.UpperLeft;
                sceneViewBox.fontSize = 12;
            }

            if (greenIconContent == null)
            {
                greenIconContent = new GUIContent(RGSKEditorSettings.Instance.greenIcon);
            }

            if (redIconContent == null)
            {
                redIconContent = new GUIContent(RGSKEditorSettings.Instance.redIcon);
            }

            if (yellowIconContent == null)
            {
                yellowIconContent = new GUIContent(RGSKEditorSettings.Instance.yellowIcon);
            }

            if (nullIconContent == null)
            {
                nullIconContent = new GUIContent(RGSKEditorSettings.Instance.nullIcon);
            }

            if (menuIconContent == null)
            {
                menuIconContent = new GUIContent(RGSKEditorSettings.Instance.menuIcon);
            }
        }
    }
}