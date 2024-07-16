using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

namespace RGSK.Editor
{
    public class EditorHelper : UnityEditor.Editor
    {
        public const string SceneSetupPath = "GameObject/RGSK/Scene Setup";
        public const string VehicleSetupPath = "GameObject/RGSK/Vehicle Setup";
        const string LastSaveDirectoryKey = "RGSK_LastSaveDir_{0}";

        [MenuItem(SceneSetupPath, false, 100)]
        public static void CreateSceneSetupWizard()
        {
            if (FindObjectOfType<RGSKSceneSetup>())
            {
                SelectObject(FindObjectOfType<RGSKSceneSetup>().gameObject);
                return;
            }

            var go = new GameObject("[RGSK]");
            go.AddComponent<RGSKSceneSetup>();
            go.transform.SetAsLastSibling();
            Undo.RegisterCreatedObjectUndo(go, "created_scene_setup");
            SelectObject(go);
        }

        [MenuItem(VehicleSetupPath, false, 101)]
        public static void CreateVehicleSetupWizard()
        {
            var selection = Selection.activeGameObject;

            if (selection != null)
            {
                if (!selection.TryGetComponent<VehicleSetup>(out var vs))
                {
                    Undo.AddComponent(selection, typeof(VehicleSetup));
                    EditorHelper.SelectObject(selection);
                }
            }
            else
            {
                Logger.Log("Please select the vehicle!");
            }
        }

        public static GameObject CreatePrefab(GameObject template, Transform parent,
                                            Vector3? pos = null, Quaternion? rot = null,
                                            bool prefab = false, bool ping = true)
        {
            var t = !prefab ? Instantiate(template) : (GameObject)PrefabUtility.InstantiatePrefab(template);
            t.name = t.name.Replace("(Clone)", "").Trim();
            t.transform.SetParent(parent, false);

            if (pos.HasValue)
            {
                t.transform.position = pos.Value;
            }

            if (rot.HasValue)
            {
                t.transform.rotation = rot.Value;
            }

            if (rot != null)
            {

            }

            if (ping)
            {
                EditorHelper.SelectObject(t, true);
            }

            return t;
        }

        public static void SelectObject(Object obj, bool ping = true)
        {
            if (obj != null)
            {
                Selection.activeObject = obj;

                if (ping)
                {
                    EditorGUIUtility.PingObject(obj);
                }
            }
        }

        public static void DrawAddToListUI<T>(List<T> list, T obj)
        {
            var added = list.Contains(obj);
            var status = added ? $"In Global [Index: {list.IndexOf(obj)}]" : "Not In Global";

            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    var content = new GUIContent();
                    content.text = status;
                    content.image = !added ? CustomEditorStyles.redIconContent.image :
                                             CustomEditorStyles.greenIconContent.image;

                    EditorGUILayout.LabelField(content);

                    if (added)
                    {
                        if (GUILayout.Button("Remove"))
                        {
                            list.Remove(obj);
                            EditorHelper.MarkSettingsAsDirty();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Add"))
                        {
                            list.Add(obj);
                            EditorHelper.MarkSettingsAsDirty();
                        }
                    }
                }
            }
        }

        public static void DrawIntegrationUI(string name, string symbol, bool added)
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    var content = new GUIContent();
                    content.text = name;
                    content.image = !added ? CustomEditorStyles.redIconContent.image :
                                             CustomEditorStyles.greenIconContent.image;

                    EditorGUILayout.LabelField(content);

                    EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                    {
                        if (!added)
                        {
                            if (GUILayout.Button("Add", GUILayout.MinWidth(200), GUILayout.MaxWidth(200)))
                            {
                                if (EditorUtility.DisplayDialog("Integration", $"Add support for {name}?",
                        "Yes", "No"))
                                {
                                    AddDefineSymbol(symbol);
                                }
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Remove", GUILayout.MinWidth(200), GUILayout.MaxWidth(200)))
                            {
                                if (EditorUtility.DisplayDialog("Integration", $"Remove support for {name}?",
                        "Yes", "No"))
                                {
                                    RemoveDefineSymbol(symbol);
                                }
                            }
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
        }

        public static void DrawItemUI(Sprite tex, string title, string description, string button1 = "", string button2 = "", System.Action onButton1Click = null, System.Action onButton2Click = null)
        {
            using (new GUILayout.VerticalScope("Box", GUILayout.MaxWidth(350), GUILayout.MinHeight(200)))
            {
                GUILayout.Label(tex ? tex.texture : CustomEditorStyles.nullIconContent.image, CustomEditorStyles.menuLabelCenter, GUILayout.MinWidth(150), GUILayout.MinHeight(150));
                EditorGUILayout.LabelField($"<b>{title}</b>", CustomEditorStyles.menuLabelLeft);
                EditorGUILayout.LabelField(description, CustomEditorStyles.menuDescription);

                using (new GUILayout.HorizontalScope())
                {
                    if (!string.IsNullOrWhiteSpace(button1))
                    {
                        if (GUILayout.Button(button1))
                        {
                            onButton1Click?.Invoke();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(button2))
                    {
                        if (GUILayout.Button(button2))
                        {
                            onButton2Click?.Invoke();
                        }
                    }
                }
            }
        }

        public static void AddLayersToProjectSettings()
        {
            if (!EditorUtility.DisplayDialog("Add Layers", $"Proceeding will add RGSK layers to the project.",
                        "Continue", "Cancel"))
            {
                return;
            }

            var layers = new string[]
            {
                "RGSK_Vehicle",
                "RGSK_Ghost",
                "RGSK_Minimap",
                "RGSK_Obstacle",
                "RGSK_PostProcessing",
            };

            foreach (var layer in layers)
            {
                CreateLayer(layer);
            }
        }

        public static void AddDemoScenesToBuilds()
        {
            if (!EditorUtility.DisplayDialog("Add Demo Scenes", $"Proceeding will add all demo scenes to the build settings.",
                        "Continue", "Cancel"))
            {
                return;
            }

            var pathList = new List<string>();

            foreach (var p in RGSKEditorSettings.Instance.demoScenes)
            {
                pathList.Add(p.ScenePath);
            }

            if (pathList.Count > 0)
            {
                var sceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

                foreach (var path in pathList)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        var index = SceneUtility.GetBuildIndexByScenePath(path);

                        if (index < 0)
                        {
                            sceneList.Add(new EditorBuildSettingsScene(path, true));
                        }
                    }
                }

                EditorBuildSettings.scenes = sceneList.ToArray();

                if (!EditorUtility.DisplayDialog("Demos", $"Demo scenes were added to your project's build settings.", "OK", "Open Demo Hub"))
                {
                    EditorSceneManager.OpenScene(pathList[0]);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Demos", "Demo scenes could not be found! Please consider reimporting the asset.", "OK");
            }
        }

        public static void DrawHeader(float width1, float width2, bool showVersion = true)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                var header = RGSKEditorSettings.Instance.welcomeHeader;

                if (header != null)
                {
                    var aspect = (float)header.width / header.height;
                    var texWidth = width1;
                    var texHeight = texWidth / aspect;
                    GUILayout.Label(header, CustomEditorStyles.menuLabelCenter,
                                    GUILayout.Width(texWidth),
                                    GUILayout.Height(texHeight),
                                    GUILayout.Width(width2),
                                    GUILayout.MaxHeight(200));
                }

                if (showVersion)
                {
                    EditorGUILayout.LabelField($"v{RGSKCore.Instance.versionNumber}", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        public static void MarkPrefabAsDirty()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }

        public static void MarkSettingsAsDirty()
        {
            EditorUtility.SetDirty(RGSKCore.Instance.ContentSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.RaceSettings);
        }

        public static void DrawLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static void DrawLabelWithinDistance(Vector3 position, string text, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text);
            }
        }

        public static void DrawLabelWithinDistance(Vector3 position, Texture2D text, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text);
            }
        }

        public static void DrawLabelWithinDistance(Vector3 position, string text, GUIStyle style, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text, style);
            }
        }

        public static bool IsSceneViewCameraInRange(Vector3 position, float distance)
        {
            Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
            return ((cameraPos.x >= 0) &&
            (cameraPos.x <= Camera.current.pixelWidth) &&
            (cameraPos.y >= 0) &&
            (cameraPos.y <= Camera.current.pixelHeight) &&
            (cameraPos.z > 0) &&
            (cameraPos.z < distance));
        }

        public static string SaveStartPath()
        {
            var dir = EditorPrefs.GetString(string.Format(LastSaveDirectoryKey, GetProjectName()), "Assets");

            if (!Directory.Exists(dir))
            {
                dir = "Assets";
            }

            return dir;
        }

        public static bool IsValidSavePath(string path, out string newPath)
        {
            newPath = path;

            if (newPath.Length == 0)
                return false;

            try
            {
                newPath = newPath.Substring(newPath.IndexOf("Assets/"));
            }
            catch
            {
                Logger.LogWarning("Please save the file under the 'Assets' directory!");
                return false;
            }

            var fileInfo = new FileInfo(path);
            EditorPrefs.SetString(string.Format(LastSaveDirectoryKey, GetProjectName()), fileInfo.DirectoryName);

            return true;
        }

        public static string GetProjectName()
        {
            var path = Application.dataPath;
            var dir = Directory.GetParent(path).FullName;
            return new DirectoryInfo(dir).Name;
        }

        public static ScriptableObject CloneScriptableObject(ScriptableObject so)
        {
            if (so == null)
                return null;

            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"{so.name}_Clone", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var clone = ScriptableObject.Instantiate(so);

                AssetDatabase.CreateAsset(clone, path);
                AssetDatabase.SaveAssets();
                EditorHelper.SelectObject(AssetDatabase.LoadMainAssetAtPath(path));

                return clone;
            }

            return null;
        }

        public static Texture CreateEmptyTexture(Color col, int width = 25, int height = 25)
        {
            var tex = new Texture2D(width, height);
            var pixels = tex.GetPixels();

            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = col;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        public static bool CheckNamespacesExists(string namepsace)
        {
            HashSet<string> existingIdentifiers = new HashSet<string>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                System.Reflection.Assembly assembly = assemblies[i];

                System.Type[] types = assembly.GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    System.Type type = types[j];
                    var typeNamespace = type.Namespace;

                    if (namepsace == typeNamespace)
                    {
                        if (!existingIdentifiers.Contains(typeNamespace))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static void AddDefineSymbol(string symbol)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();

            if (allDefines.Contains(symbol))
                return;

            allDefines.Add(symbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public static void RemoveDefineSymbol(string symbol)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();

            if (!allDefines.Contains(symbol))
                return;

            allDefines.Remove(symbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public static bool EVPSupport()
        {
#if EVP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool RCCSupport()
        {
#if RCC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool RCCProSupport()
        {
#if RCC_PRO_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool VPPSupport()
        {
#if VPP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool NWH2Support()
        {
#if NWH2_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool HSCSupport()
        {
#if HSC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool ASHVPSupport()
        {
#if ASHVP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool UVCSupport()
        {
#if UVC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        #region Tags & Layers
        static int maxTags = 10000;
        static int maxLayers = 31;
        static int startLayerIndex = 6;

        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <returns><c>true</c>, if tag was added, <c>false</c> otherwise.</returns>
        /// <param name="tagName">Tag name.</param>
        public static bool CreateTag(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            if (tagsProp.arraySize >= maxTags)
            {
                Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
                return false;
            }
            // if not found, add it
            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;
                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                // Set array element to tagName
                sp.stringValue = tagName;
                Debug.Log("Tag: " + tagName + " has been added");
                // Save settings
                tagManager.ApplyModifiedProperties();

                return true;
            }
            else
            {
                //Debug.Log ("Tag: " + tagName + " already exists");
            }
            return false;
        }

        public static string NewTag(string name)
        {
            CreateTag(name);

            if (name == null || name == "")
            {
                name = "Untagged";
            }

            return name;
        }

        /// <summary>
        /// Removes the tag.
        /// </summary>
        /// <returns><c>true</c>, if tag was removed, <c>false</c> otherwise.</returns>
        /// <param name="tagName">Tag name.</param>
        ///
        public static bool RemoveTag(string tagName)
        {

            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            if (PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                SerializedProperty sp;

                for (int i = 0, j = tagsProp.arraySize; i < j; i++)
                {

                    sp = tagsProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == tagName)
                    {
                        tagsProp.DeleteArrayElementAtIndex(i);
                        Debug.Log("Tag: " + tagName + " has been removed");
                        // Save settings
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }

                }
            }

            return false;

        }

        /// <summary>
        /// Checks to see if tag exists.
        /// </summary>
        /// <returns><c>true</c>, if tag exists, <c>false</c> otherwise.</returns>
        /// <param name="tagName">Tag name.</param>
        public static bool TagExists(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layers Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            return PropertyExists(tagsProp, 0, maxTags, tagName);
        }
        /// <summary>
        /// Adds the layer.
        /// </summary>
        /// <returns><c>true</c>, if layer was added, <c>false</c> otherwise.</returns>
        /// <param name="layerName">Layer name.</param>
        public static bool CreateLayer(string layerName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            if (!PropertyExists(layersProp, 0, maxLayers, layerName))
            {
                SerializedProperty sp;
                for (int i = startLayerIndex, j = maxLayers; i < j; i++)
                {
                    sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == "")
                    {
                        // Assign string value to layer
                        sp.stringValue = layerName;
                        Debug.Log("Layer: " + layerName + " has been added");
                        // Save settings
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }
                    if (i == j)
                        Debug.Log("All allowed layers have been filled");
                }
            }
            else
            {
                //Debug.Log ("Layer: " + layerName + " already exists");
            }
            return false;
        }

        public static string NewLayer(string name)
        {
            if (name != null || name != "")
            {
                CreateLayer(name);
            }

            return name;
        }

        /// <summary>
        /// Removes the layer.
        /// </summary>
        /// <returns><c>true</c>, if layer was removed, <c>false</c> otherwise.</returns>
        /// <param name="layerName">Layer name.</param>
        public static bool RemoveLayer(string layerName)
        {

            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Tags Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (PropertyExists(layersProp, 0, layersProp.arraySize, layerName))
            {
                SerializedProperty sp;

                for (int i = 0, j = layersProp.arraySize; i < j; i++)
                {

                    sp = layersProp.GetArrayElementAtIndex(i);

                    if (sp.stringValue == layerName)
                    {
                        sp.stringValue = "";
                        Debug.Log("Layer: " + layerName + " has been removed");
                        // Save settings
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }

                }
            }

            return false;

        }
        /// <summary>
        /// Checks to see if layer exists.
        /// </summary>
        /// <returns><c>true</c>, if layer exists, <c>false</c> otherwise.</returns>
        /// <param name="layerName">Layer name.</param>
        public static bool LayerExists(string layerName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            return PropertyExists(layersProp, 0, maxLayers, layerName);
        }
        /// <summary>
        /// Checks if the value exists in the property.
        /// </summary>
        /// <returns><c>true</c>, if exists was propertyed, <c>false</c> otherwise.</returns>
        /// <param name="property">Property.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="value">Value.</param>
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}