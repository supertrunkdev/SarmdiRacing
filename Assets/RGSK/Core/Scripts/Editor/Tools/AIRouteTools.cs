using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace RGSK.Editor
{
    [EditorTool("Insert Speedzones", typeof(AIRoute))]
    public class AISpeedzoneTool : EditorTool
    {
        AIRoute _route;
        GUIContent _iconContent;

        public override GUIContent toolbarIcon => _iconContent;

        void OnEnable() 
        {
            _iconContent = new GUIContent()
            {
                image = RGSKEditorSettings.Instance.aiRouteSpeedzoneToolIcon,
                text = "Insert Speedzones",
                tooltip = "Insert Speedzones"
            };
        }

        void OnDisable() 
        {
            if(_route != null)
            {
                _route.insertMode = AIRouteInsertMode.Default;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView))
                return;

            foreach (var t in targets)
            {
                if(!(t is AIRoute route))
                    continue;

                _route = route;
                _route.insertMode = AIRouteInsertMode.Speedzones;
            }
        }
    }

    [EditorTool("Insert Racing Line", typeof(AIRoute))]
    public class AIRacingLineTool : EditorTool
    {
        AIRoute _route;
        GUIContent _iconContent;

        public override GUIContent toolbarIcon => _iconContent;

        void OnEnable() 
        {
            ToolManager.activeToolChanged += ActiveToolChanged;

            _iconContent = new GUIContent()
            {
                image = RGSKEditorSettings.Instance.aiRouteRacinglineToolIcon,
                text = "Insert Racing Line",
                tooltip = "Insert Racing Line"
            };
        }

        void OnDisable() 
        {
            ToolManager.activeToolChanged -= ActiveToolChanged;

            if(_route != null)
            {
                _route.insertMode = AIRouteInsertMode.Default;
            }
        }

        void ActiveToolChanged()
        {
            if(_route != null)
            {
                _route.insertMode = AIRouteInsertMode.Default;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView))
                return;

            foreach (var t in targets)
            {
                if(!(t is AIRoute route))
                    continue;

                _route = route;
                _route.insertMode = AIRouteInsertMode.RacingLine;
            }
        }
    }
}