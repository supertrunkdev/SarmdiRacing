using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class InputActionBindingUI : MonoBehaviour
    {
        [SerializeField] InputActionReference inputAction;
        [SerializeField] TMP_Text actionText;
        [SerializeField] TMP_Text bindingText;
        [SerializeField] int bindIndex;

        [Header("Gamepad")]
        [SerializeField] GamepadIconDefinition gamepadIcons;
        [SerializeField] Image gamepadIcon;
        [SerializeField] bool useGamepadIcons = true;

        void OnEnable()
        {
            RGSKEvents.OnInputDeviceChanged.AddListener(OnInputDeviceChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnInputDeviceChanged.RemoveListener(OnInputDeviceChanged);
        }

        void Awake()
        {
            OnInputDeviceChanged(InputManager.Instance.ActiveInputDevice);
        }

        void OnInputDeviceChanged(InputDevice device)
        {
            if (inputAction == null)
                return;

            var usingKeyboard = InputManager.Instance.ActiveController == InputManager.InputController.MouseAndKeyboard;

            SetActionText(inputAction.action.name);
            SetBindingText($"[{GetBindingText(inputAction, usingKeyboard ? "Keyboard" : "Gamepad")}]");
            SetIcon(null);

            if (!usingKeyboard && useGamepadIcons)
            {
                SetBindingText("");
                SetIcon(GetGamepadIcon(inputAction, InputManager.Instance.ActiveController));
            }

            if (GeneralHelper.IsMobilePlatform())
            {
                SetBindingText("");
                SetIcon(null);
            }
        }

        public void SetActionText(string value) => actionText?.SetText(value);
        public void SetBindingText(string value) => bindingText?.SetText(value);

        public void SetIcon(Sprite icon)
        {
            if (gamepadIcons == null || gamepadIcon == null)
                return;

            gamepadIcon.sprite = icon;
            gamepadIcon.DisableIfNullSprite();
        }

        string GetBindingText(InputAction a, string mask)
        {
            var index = a.GetBindingIndex(InputBinding.MaskByGroup(mask));

            if (index == -1)
            {
                return null;
            }

            if (a.bindings[index].isPartOfComposite)
            {
                index += bindIndex;
            }

            return a.GetBindingDisplayString(index, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }

        Sprite GetGamepadIcon(InputAction a, InputManager.InputController controller)
        {
            var index = a.GetBindingIndex(InputBinding.MaskByGroup("Gamepad"));

            if (index == -1)
            {
                return null;
            }

            if (a.bindings[index].isPartOfComposite)
            {
                index += bindIndex;
            }

            var bind = a.GetBindingDisplayString(index, out var deviceLayout, out var controlPath);
            var icons = gamepadIcons.GetIconSet(controller);

            if (icons != null)
            {
                return icons.GetSprite(controlPath);
            }

            return null;
        }
    }
}