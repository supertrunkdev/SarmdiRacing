using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using RGSK;

namespace RGSK
{
    public class InputManager : Singleton<InputManager>, InputActions.IVehicleActions, InputActions.ICameraActions, InputActions.IUIActions
    {
        public enum InputMode
        {
            Disabled,
            Gameplay,
            UI,
            Replay,
        }

        public enum InputController
        {
            MouseAndKeyboard,
            Xbox,
            PS4,
            PS5,
        }

        #region Events
        //Vehicle
        public static event UnityAction<float> ThrottleEvent;
        public static event UnityAction<float> SteerEvent;
        public static event UnityAction<float> BrakeEvent;
        public static event UnityAction<float> HandBrakeEvent;
        public static event UnityAction<float> BoostEvent;
        public static event UnityAction ShiftUpEvent;
        public static event UnityAction ShiftDownEvent;
        public static event UnityAction HornStartEvent;
        public static event UnityAction HornCancelEvent;
        public static event UnityAction RepositionStartEvent;
        public static event UnityAction RepositionCancelEvent;
        public static event UnityAction RepositionPerformedEvent;
        public static event UnityAction HeadlightEvent;
        public static event UnityAction EngineToggleEvent;

        //Camera
        public static event UnityAction<int> ChangeCameraPerspectiveEvent;
        public static event UnityAction<int> ChangeCameraTargetEvent;
        public static event UnityAction<Vector2> CameraLookEvent;
        public static event UnityAction<float> CameraLookBackEvent;

        //Pause
        public static event UnityAction PauseEvent;

        //UI
        public static event UnityAction MenuBackEvent;
        public static event UnityAction<int> TabChangedEvent;
        #endregion

        public InputMode ActiveInputMode { get; private set; }
        public InputMode PreviousInputMode { get; private set; }
        public InputDevice ActiveInputDevice { get; private set; }
        public InputController ActiveController { get; private set; }

        InputActions _actions;

        public override void Awake()
        {
            base.Awake();

            if (_actions == null)
            {
                _actions = new InputActions();
                _actions.Vehicle.SetCallbacks(this);
                _actions.Camera.SetCallbacks(this);
                _actions.UI.SetCallbacks(this);
            }

            SetInputMode(InputMode.Gameplay);
        }

        void OnEnable()
        {
            InputSystem.onActionChange += OnInputDeviceChanged;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= OnInputDeviceChanged;
        }

        public void SetInputMode(InputMode mode)
        {
            switch (mode)
            {
                case InputMode.Disabled:
                    {
                        _actions.Vehicle.Disable();
                        _actions.Camera.Disable();
                        _actions.UI.Disable();
                        break;
                    }

                case InputMode.Gameplay:
                    {
                        _actions.Vehicle.Enable();
                        _actions.Camera.Enable();
                        _actions.UI.Disable();
                        break;
                    }

                case InputMode.UI:
                    {
                        _actions.Vehicle.Disable();
                        _actions.Camera.Disable();
                        _actions.UI.Enable();
                        break;
                    }

                case InputMode.Replay:
                    {
                        _actions.Vehicle.Disable();
                        _actions.Camera.Enable();
                        _actions.UI.Enable();
                        break;
                    }
            }

            PreviousInputMode = ActiveInputMode;
            ActiveInputMode = mode;
        }

        void OnInputDeviceChanged(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction)obj;
                var lastDevice = inputAction.activeControl.device;

                if (lastDevice != ActiveInputDevice)
                {
                    ActiveInputDevice = lastDevice;
                    UpdateActiveController();
                    RGSKEvents.OnInputDeviceChanged?.Invoke(ActiveInputDevice);
                }
            }
        }

        void UpdateActiveController()
        {
            if (ActiveInputDevice.displayName.Contains("Mouse") ||
               ActiveInputDevice.displayName.Contains("Keyboard"))
            {
                ActiveController = InputController.MouseAndKeyboard;
            }

            if (ActiveInputDevice.displayName.Contains("Xbox"))
            {
                ActiveController = InputController.Xbox;
            }

            if (ActiveInputDevice.displayName.Contains("DualShock"))
            {
                ActiveController = InputController.PS4;
            }

            if (ActiveInputDevice.displayName.Contains("DualSense"))
            {
                ActiveController = InputController.PS5;
            }
        }

        public void Rumble(float lowFrequency, float highFrequency, float duration)
        {
            if (!RGSKCore.Instance.InputSettings.vibrate)
                return;

            if (ActiveController == InputController.MouseAndKeyboard)
                return;

            if (Gamepad.current != null)
            {
                StartCoroutine(RumbleRoutine(Gamepad.current, lowFrequency, highFrequency, duration));
            }
        }

        IEnumerator RumbleRoutine(Gamepad pad, float lowFrequency, float highFrequency, float duration)
        {
            if (duration <= 0)
            {
                pad.SetMotorSpeeds(0f, 0f);
                yield break;
            }

            pad.SetMotorSpeeds(lowFrequency, highFrequency);

            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            pad.SetMotorSpeeds(0f, 0f);
        }

        #region Vehicle Input
        public void OnThrottle(InputAction.CallbackContext context)
        {
            ThrottleEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnBrake(InputAction.CallbackContext context)
        {
            BrakeEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnSteer(InputAction.CallbackContext context)
        {
            SteerEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnHandbrake(InputAction.CallbackContext context)
        {
            HandBrakeEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnShiftDown(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                ShiftDownEvent?.Invoke();
        }

        public void OnShiftUp(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                ShiftUpEvent?.Invoke();
        }

        public void OnHorn(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                HornStartEvent?.Invoke();

            if (context.phase == InputActionPhase.Canceled)
                HornCancelEvent?.Invoke();
        }

        public void OnBoost(InputAction.CallbackContext context)
        {
            BoostEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnEngine(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                EngineToggleEvent?.Invoke();
        }

        public void OnHeadlights(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                HeadlightEvent?.Invoke();
        }

        public void OnReposition(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    {
                        RepositionStartEvent?.Invoke();
                        break;
                    }

                case InputActionPhase.Canceled:
                    {
                        RepositionCancelEvent?.Invoke();
                        break;
                    }

                case InputActionPhase.Performed:
                    {
                        RepositionPerformedEvent?.Invoke();
                        break;
                    }
            }
        }
        #endregion

        #region Camera Input
        public void OnChangePerspective(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                ChangeCameraPerspectiveEvent?.Invoke((int)context.ReadValue<float>());
        }

        public void OnChangeTarget(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                ChangeCameraTargetEvent?.Invoke((int)context.ReadValue<float>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            CameraLookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLookBack(InputAction.CallbackContext context)
        {
            CameraLookBackEvent?.Invoke(context.ReadValue<float>());
        }
        #endregion

        #region UI Input
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                PauseEvent?.Invoke();
        }

        public void OnChangeTab(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                TabChangedEvent?.Invoke((int)context.ReadValue<float>());
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {

        }

        public void OnSelect(InputAction.CallbackContext context)
        {

        }

        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MenuBackEvent?.Invoke();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {

        }

        public void OnClick(InputAction.CallbackContext context)
        {

        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {

        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {

        }

        public void OnRightClick(InputAction.CallbackContext context)
        {

        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {

        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {

        }
        #endregion
    }
}