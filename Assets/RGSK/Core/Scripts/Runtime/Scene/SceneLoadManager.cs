using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RGSK.Helpers;

namespace RGSK
{
    public class SceneLoadManager : MonoBehaviour
    {
        public static string SceneToLoad;
        public static float LoadingProgress { get; private set; }

        void Start()
        {
            if (!string.IsNullOrWhiteSpace(SceneToLoad))
            {
                StartCoroutine(LoadSceneAsync(SceneToLoad));
            }
        }

        public static void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(RGSKCore.Instance.SceneSettings.loadingScene))
            {
                Logger.Log("A loading scene has not been assigned! Please assign one in the RGSK Settings Window");
                return;
            }

            SceneToLoad = sceneName;
            Application.backgroundLoadingPriority = RGSKCore.Instance.SceneSettings.backgroundLoadingPriority;
            SceneManager.LoadScene(RGSKCore.Instance.SceneSettings.loadingScene);
        }

        public static void LoadScene(SceneReference scene)
        {
            if (string.IsNullOrEmpty(scene))
            {
                Logger.Log("The scene reference you are trying to load has not been assigned!");
                return;
            }

            LoadScene(scene.ScenePath);
        }

        public static void ReloadScene() => LoadScene(SceneManager.GetActiveScene().name);

        public static void LoadMainScene() => LoadScene(RGSKCore.Instance.SceneSettings.mainMenuScene);

        public static void QuitApplication()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            LoadingProgress = 0;
            RGSKEvents.OnBeforeSceneLoad.Invoke();
            yield return null;

            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                LoadingProgress = operation.progress;
                yield return null;
            }

            LoadingProgress = 1;

            yield return GeneralHelper.GetCachedWaitForSeconds(RGSKCore.Instance.SceneSettings.loadCompleteDelay);

            operation.allowSceneActivation = true;
            RGSKEvents.OnAfterSceneLoad.Invoke();
        }
    }
}