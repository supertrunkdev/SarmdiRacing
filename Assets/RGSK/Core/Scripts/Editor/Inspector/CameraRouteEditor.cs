using UnityEngine;
using UnityEditor;
using Cinemachine;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CameraRoute))]
    public class CameraRouteEditor : UnityEditor.Editor
    {
        CameraRoute _target;
        string[] tabs = new string[] { "Look At", "Fixed", "Dolly" };
        static int tabIndex;

        void OnEnable()
        {
            _target = (CameraRoute)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use the scene-view camera to position and then click on the 'create' button to place a camera", MessageType.Info);
            tabIndex = GUILayout.Toolbar(tabIndex, tabs, CustomEditorStyles.horizontalToolbarButton);
            GameObject template = null;

            if (GUILayout.Button("Create", GUILayout.Height(25)))
            {
                switch (tabIndex)
                {
                    case 0:
                        {
                            template = RGSKEditorSettings.Instance.lookAtCameraTemplate;
                            break;
                        }

                    case 1:
                        {
                            template = RGSKEditorSettings.Instance.fixedCameraTemplate;
                            break;
                        }

                    case 2:
                        {
                            template = RGSKEditorSettings.Instance.dollyCameraTemplate;
                            break;
                        }
                }
            }

            if (template != null && SceneView.lastActiveSceneView.camera != null)
            {
                var cam = EditorHelper.CreatePrefab(template, _target.transform, Vector3.zero, Quaternion.identity, false, false);
                var pos = SceneView.lastActiveSceneView.camera.transform.position;
                var rot = SceneView.lastActiveSceneView.camera.transform.rotation;
                var rc = cam.GetComponentInChildren<RouteCamera>();
                var path = cam.GetComponentInChildren<CinemachineSmoothPath>();
                
                if (rc != null)
                {
                    rc.transform.SetPositionAndRotation(pos, rot);
                }

                if (path != null)
                {
                    path.transform.SetPositionAndRotation(pos, Quaternion.identity);
                }

                if (_target.route != null && _target.route.nodes.Count > 1)
                {
                    if (rc != null)
                    {
                        rc.routeDistance = _target.route.GetDistanceAtPosition(pos);
                    }
                }

                Undo.RegisterCreatedObjectUndo(cam, "created_cinematiccamera");
            }

            if (GUI.changed)
            {
                _target.GetCameras();
            }

            EditorHelper.DrawLine();
            DrawDefaultInspector();
        }
    }
}