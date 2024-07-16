using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class ReplayScreen : UIScreen
    {
        [SerializeField] CanvasGroup uiCanvasGroup;
        [SerializeField] Slider slider;
        [SerializeField] TMP_Text currentTimeText;
        [SerializeField] TMP_Text totalTimeText;
        [SerializeField] TMP_Text playbackSpeedText;
        [SerializeField] Button playAndPauseButton;
        [SerializeField] Button fastForwardButton;
        [SerializeField] Button rewindButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button framePlaybackButton;
        [SerializeField] Button slowMotionButton;
        [SerializeField] Button hideUiButton;
        [SerializeField] Button exitButton;
        [SerializeField] Button changeCameraButton;
        [SerializeField] Button changeTargetNextButton;
        [SerializeField] Button changeTargetPreviousButton;
        [SerializeField] float autoUIHideTimeout = 30;

        bool _sliderPressed;
        float _lastUIHideTime;
        float _autoUIHideTimer;

        public override void Initialize()
        {
            base.Initialize();

            playAndPauseButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.TogglePlayAndPause());
            fastForwardButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.FastForward());
            rewindButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.Rewind());
            restartButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.RestartPlayback());
            framePlaybackButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.FrameByFramePlayback());
            slowMotionButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.ToggleSlowMotion());
            hideUiButton?.onClick.AddListener(() => ToggleUI(false));
            exitButton?.onClick.AddListener(() => Back());
            changeCameraButton?.onClick.AddListener(() => CameraManager.Instance?.ChangePerspective(1));
            changeTargetNextButton?.onClick.AddListener(() => ChangeTarget(1));
            changeTargetPreviousButton?.onClick.AddListener(() => ChangeTarget(-1));

            if (slider != null)
            {
                var eventTrigger = slider.gameObject.GetOrAddComponent<EventTrigger>();
                eventTrigger.AddListener(EventTriggerType.PointerDown, OnSliderDown);
                eventTrigger.AddListener(EventTriggerType.PointerUp, OnSliderUp);
                slider.wholeNumbers = true;
            }
        }

        public override void Open()
        {
            base.Open();

            RGSKCore.Instance.UISettings.showNameplates = false;
            CameraManager.Instance?.ToggleRouteCameras(true);

            if (RaceManager.Instance.Initialized)
            {
                RaceManager.Instance.ForceFinishRace(true);
            }
        }

        public override void Close()
        {
            base.Close();

            RecorderManager.Instance?.ReplayRecorder.SetPlaybackSpeed(1);
            RecorderManager.Instance?.ReplayRecorder.ActivateSlowMotionPlayback(false);
            CameraManager.Instance?.ToggleRouteCameras(RaceManager.Instance.Initialized);
            RGSKCore.Instance.UISettings.showNameplates = SaveData.Instance.gameSettingsData.showNameplates;
        }

        public override void Back()
        {
            base.Back();

            if (RaceManager.Instance.Initialized && RaceManager.Instance.CurrentState == RaceState.PostRace)
                return;

            RecorderManager.Instance?.ReplayRecorder?.StopPlayback();
        }

        void Update()
        {
            if (!IsOpen())
                return;

            UpdateUIElements();

            if (InputHelper.MouseMoved() ||
                InputHelper.KeyboardPressed() ||
                InputHelper.GamepadButtonOrAxisPressed() ||
                InputHelper.ScreenTouched())
            {
                _autoUIHideTimer = 0;

                if (Time.unscaledTime > _lastUIHideTime + 0.5f)
                {
                    ToggleUI(true);
                }
            }
            else
            {
                if (uiCanvasGroup != null && uiCanvasGroup.alpha > 0)
                {
                    _autoUIHideTimer += Time.unscaledDeltaTime;
                    if (_autoUIHideTimer > autoUIHideTimeout)
                    {
                        ToggleUI(false);
                    }
                }
            }
        }

        void UpdateUIElements()
        {
            if (RecorderManager.Instance.ReplayRecorder == null || !RecorderManager.Instance.ReplayRecorder.IsPlayback)
                return;

            currentTimeText?.SetText(UIHelper.FormatTimeText(RecorderManager.Instance.ReplayRecorder.GetCurrentTime, TimeFormat.MM_SS));
            totalTimeText?.SetText(UIHelper.FormatTimeText(RecorderManager.Instance.ReplayRecorder.GetTotalTime, TimeFormat.MM_SS));

            if (!RecorderManager.Instance.ReplayRecorder.IsSlowMotion)
            {
                playbackSpeedText?.SetText($"{RecorderManager.Instance.ReplayRecorder.PlaybackSpeed}x");
            }
            else
            {
                playbackSpeedText?.SetText($"{Time.timeScale}x");
            }

            if (slider != null)
            {
                slider.maxValue = RecorderManager.Instance.ReplayRecorder.GetTotalFrames();

                if (!_sliderPressed)
                {
                    slider.value = RecorderManager.Instance.ReplayRecorder.CurrentFrame;
                }
            }
        }

        public void ToggleUI(bool visible)
        {
            uiCanvasGroup?.SetAlpha(visible ? 1 : 0);

            if (!visible)
            {
                _lastUIHideTime = Time.unscaledTime;
            }
        }

        void OnSliderDown(PointerEventData data)
        {
            _sliderPressed = true;
        }

        void OnSliderUp(PointerEventData data)
        {
            RecorderManager.Instance.ReplayRecorder.PlayFrame((int)slider.value);
            _sliderPressed = false;
        }

        void ChangeTarget(int dir)
        {
            //Fix for an issue that stops target changing when playback is paused
            StartCoroutine(ChangeTargetRoutine(dir));
        }

        IEnumerator ChangeTargetRoutine(int dir)
        {
            var oldTimeScale = Time.timeScale;

            Time.timeScale = 1;
            CameraManager.Instance?.ChangeTarget(dir);

            yield return null;

            Time.timeScale = oldTimeScale;
        }
    }
}