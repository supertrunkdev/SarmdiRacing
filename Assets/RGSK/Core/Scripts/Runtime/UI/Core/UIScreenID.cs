using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/UI/UI Screen ID")]
    public class UIScreenID : ScriptableObject
    {
        public UIScreen screenPrefab;
        public UIScreen screenPrefabMobile;
        public InputManager.InputMode onOpenInputMode = InputManager.InputMode.UI;
        public bool isPersistentScreen;

        public void Open()
        {
            UIManager.Instance?.OpenScreen(this);
        }

        public void Close()
        {
            UIManager.Instance?.CloseScreen(this);
        }
    }
}