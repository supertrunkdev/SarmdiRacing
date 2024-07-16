using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class UIManager : Singleton<UIManager>
    {
        public UIScreen ActiveScreen { get; private set; }
        Dictionary<UIScreenID, UIScreen> _screenList = new Dictionary<UIScreenID, UIScreen>();

        void OnEnable()
        {
            InputManager.MenuBackEvent += OpenPreviousScreen;
        }

        void OnDisable()
        {
            InputManager.MenuBackEvent -= OpenPreviousScreen;
        }

        public void RegisterScreen(UIScreen screen)
        {
            if (screen.ID == null)
            {
                Logger.LogError($"The screen {screen} has no ID!");
                return;
            }

            if (!_screenList.ContainsKey(screen.ID))
            {
                _screenList.Add(screen.ID, screen);
                screen.gameObject.SetActive(true);
                screen.Initialize();
                CloseScreen(screen.ID);
            }
        }

        public void UnregisterScreen(UIScreen screen)
        {
            if (_screenList.ContainsKey(screen.ID))
            {
                _screenList.Remove(screen.ID);
            }
        }

        public void OpenScreen(UIScreenID id, bool addToHistory = true)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to open is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                CloseAllScreens();
                screen.Open();

                if (ActiveScreen != null && addToHistory)
                {
                    screen.PreviousScreen = ActiveScreen;
                }

                ActiveScreen = screen;
            }
            else
            {
                CreateScreen(id, true);
            }
        }

        public void CloseScreen(UIScreenID id)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to close is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                if (screen.IsOpen())
                {
                    screen.Close();
                }
            }
        }

        public void CreateScreen(UIScreenID id, bool open = false)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to create is null!");
                return;
            }

            if (_screenList.ContainsKey(id))
                return;

            var prefab = id.screenPrefab;

            if (GeneralHelper.IsMobilePlatform() && id.screenPrefabMobile != null)
            {
                prefab = id.screenPrefabMobile;
            }

            if (prefab == null)
            {
                Logger.LogWarning($"The UI screen with ID \"{id}\"could not be created! Please ensure it's prefab is assigned or add it to the scene.");
                return;
            }

            var newScreen = Instantiate(prefab);

            if (id.isPersistentScreen)
            {
                newScreen.gameObject.AddComponent<DontDestroyOnLoad>();
            }
            else
            {
                newScreen.transform.SetParent(GeneralHelper.GetDynamicParent());
            }

            RegisterScreen(newScreen);

            if (open)
            {
                OpenScreen(id);
            }
        }

        public void DestroyScreen(UIScreenID id)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to destroy is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                Destroy(screen.gameObject);
            }
        }

        public void OpenPreviousScreen()
        {
            if (ModalWindowManager.Instance != null && ModalWindowManager.Instance.IsOpen())
                return;

            ActiveScreen?.Back();
        }

        public void CloseAllScreens()
        {
            foreach (var screen in _screenList.Keys)
            {
                CloseScreen(screen);
            }
        }

        public void ClearScreenHistory()
        {
            foreach (var screen in _screenList.Values)
            {
                screen.PreviousScreen = null;
            }

            ActiveScreen = null;
        }

        public bool HasScreenHistory() => ActiveScreen != null && ActiveScreen.PreviousScreen != null;
    }
}