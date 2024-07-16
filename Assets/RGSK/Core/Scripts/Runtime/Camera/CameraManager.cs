using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using RGSK.Helpers;

namespace RGSK
{
    public class CameraManager : Singleton<CameraManager>
    {
        [ExposedScriptableObject] public CameraSettings settings;
        [SerializeField] CinemachineBrain cinemachineBrain;

        public CinemachineBrain Camera => cinemachineBrain;
        public Transform Target { get; private set; }
        public RGSKEntity FocusedEntity { get; private set; }

        public static int FocusedIndex = 0;

        CameraSettings _settings => !settings ? RGSKCore.Instance.GeneralSettings.cameraSettings : settings;
        List<RGSKEntity> _targets => RGSKCore.Instance.GeneralSettings.entitySet.Items.Where(x => !x.IsVirtual).ToList();
        List<CameraPerspectiveTarget> _perspectiveTargets = new List<CameraPerspectiveTarget>();
        List<ReverseCameraTarget> _reverseCameraTargets = new List<ReverseCameraTarget>();
        List<CameraPerspective> _availablePerspectives = new List<CameraPerspective>();
        ReverseCamera _reverseCamera;
        CameraRoute _cameraRoute;
        int _perspectiveIndex;
        bool _routeCamerasEnabled;
        bool _inReplay;

        void OnEnable()
        {
            InputManager.ChangeCameraPerspectiveEvent += ChangePerspective;
            InputManager.ChangeCameraTargetEvent += ChangeTarget;
            RGSKEvents.OnEntityRemoved.AddListener(OnEntityRemoved);
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        void OnDisable()
        {
            InputManager.ChangeCameraPerspectiveEvent -= ChangePerspective;
            InputManager.ChangeCameraTargetEvent -= ChangeTarget;
            RGSKEvents.OnEntityRemoved.RemoveListener(OnEntityRemoved);
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);
        }

        public override void Awake()
        {
            base.Awake();
            CreateCameras();
        }

        IEnumerator Start()
        {
            //wait for entities to initialize
            yield return null;
            ChangeTarget(0);
        }

        void CreateCameras()
        {
            if (_settings == null)
                return;

            _perspectiveIndex = SaveData.Instance.gameSettingsData.cameraPerspectiveIndex;
            _reverseCamera = GetComponentInChildren<ReverseCamera>();

            foreach (var p in _settings.perspectives)
            {
                if (p.virtualCamera != null)
                {
                    p.runtimeCamera = Instantiate(p.virtualCamera, GeneralHelper.GetDynamicParent());
                }
            }
        }

        public void ChangePerspective(int direction)
        {
            if (_routeCamerasEnabled)
            {
                ToggleRouteCameras(false);
                _perspectiveIndex = 0;
                direction = 0;
            }

            _perspectiveIndex = GeneralHelper.ValidateIndex(_perspectiveIndex + direction, 0, _availablePerspectives.Count, true);

            if (_settings.cycleToRouteCameras || _inReplay)
            {
                if (direction > 0 && _perspectiveIndex == 0)
                {
                    ToggleRouteCameras(true);
                }
            }

            UpdatePerspective();

            if (!_inReplay)
            {
                SaveData.Instance.gameSettingsData.cameraPerspectiveIndex = _perspectiveIndex;
            }
        }

        public void ChangeTarget(int direction)
        {
            if (_targets == null || _targets.Count == 0)
                return;

            FocusedIndex = GeneralHelper.ValidateIndex(FocusedIndex + direction, 0, _targets.Count, true);
            SetTarget(_targets[FocusedIndex].transform);
        }

        public void SetTarget(Transform target)
        {
            if (_settings == null || target == null)
                return;

            Target = target;

            _perspectiveTargets = Target.GetComponentsInChildren<CameraPerspectiveTarget>().ToList();
            _reverseCameraTargets = Target.GetComponentsInChildren<ReverseCameraTarget>().ToList();

            foreach (var p in _settings.perspectives)
            {
                if (p.runtimeCamera == null)
                    continue;

                p.runtimeCamera.Follow = p.runtimeCamera.LookAt = null;

                foreach (var t in _perspectiveTargets)
                {
                    if (t.perspectives.Contains(p))
                    {
                        p.runtimeCamera.Follow = p.runtimeCamera.LookAt = t.transform;
                        break;
                    }
                }
            }

            _availablePerspectives = _settings.perspectives.Where(x => x.runtimeCamera != null && x.runtimeCamera.Follow != null).ToList();

            if (!_routeCamerasEnabled)
            {
                ChangePerspective(0);
            }

            UpdateFocusedIndex();
            RGSKEvents.OnCameraTargetChanged.Invoke(Target);
        }

        public void UpdatePerspective()
        {
            if (_settings == null || _availablePerspectives.Count == 0)
                return;

            DisableCameras();

            for (int i = 0; i < _availablePerspectives.Count; i++)
            {
                var cam = _availablePerspectives[i].runtimeCamera;
                if (i == _perspectiveIndex)
                {
                    cam.enabled = true;
                    if (cam != null && cam.Follow != null)
                    {
                        if (cam.TryGetComponent<CinemachineCameraOffset>(out var o))
                        {
                            if (cam.Follow.TryGetComponent<CameraPerspectiveTarget>(out var t))
                            {
                                o.m_Offset = t.offset;
                            }
                        }
                    }
                    break;
                }
            }

            if (_reverseCameraTargets.Count > 0)
            {
                var reverseTarget = _reverseCameraTargets.FirstOrDefault(x => x.perspective == GetCurrentPerspective());
                if (reverseTarget != null)
                {
                    _reverseCamera?.SetTarget(reverseTarget);
                }
                else
                {
                    _reverseCamera?.SetTarget(_reverseCameraTargets[0]);
                }
            }

            UpdateAudioMixerProperties(GetCurrentPerspective().volume, GetCurrentPerspective().lowPassFrequency);
            RGSKEvents.OnCameraPerspectiveChanged.Invoke(GetCurrentPerspective());
        }

        public void SetRouteCameras(CameraRoute camRoute)
        {
            _cameraRoute = camRoute;
            ToggleRouteCameras(false);
        }

        public void ToggleRouteCameras(bool toggle)
        {
            if (_cameraRoute == null)
                return;

            _cameraRoute.Toggle(toggle);
            _routeCamerasEnabled = toggle;

            if (_routeCamerasEnabled)
            {
                UpdateAudioMixerProperties(_cameraRoute.volume, _cameraRoute.lowPassFrequency);
            }
        }

        public void SetUpdateMethod(CinemachineBrain.UpdateMethod method)
        {
            Camera.m_UpdateMethod = method;
        }

        void UpdateAudioMixerProperties(float volume, float lowPassFrequency)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Mixer?.SetFloat(AudioHelper.VehicleVolumeParam, ConversionHelper.LinearToDecibel(volume));
                AudioManager.Instance.Mixer?.SetFloat(AudioHelper.VehicleLowPassParam, lowPassFrequency);
            }
        }

        void UpdateFocusedIndex()
        {
            if (Target == null)
                return;

            FocusedIndex = GeneralHelper.GetEntityIndex(Target.gameObject);
            FocusedEntity = GeneralHelper.GetEntity(FocusedIndex);
        }

        void DisableCameras()
        {
            foreach (var c in _settings.perspectives)
            {
                if (c.runtimeCamera != null)
                {
                    c.runtimeCamera.enabled = false;
                }
            }
        }

        void OnDestroy()
        {
            foreach (var p in _settings.perspectives)
            {
                if (p.runtimeCamera != null)
                {
                    p.runtimeCamera = null;
                }
            }
        }

        void OnEntityRemoved(RGSKEntity e)
        {
            UpdateFocusedIndex();
        }

        void OnReplayStart()
        {
            _inReplay = true;
        }

        void OnReplayStop()
        {
            if (!_inReplay)
                return;

            _inReplay = false;
            _perspectiveIndex = SaveData.Instance.gameSettingsData.cameraPerspectiveIndex;
            UpdatePerspective();
        }

        public CameraPerspective GetCurrentPerspective() => _availablePerspectives[_perspectiveIndex];
        public CameraPerspectiveTarget GetCurrentPerspectiveTarget() => _perspectiveTargets.FirstOrDefault(x => x.perspectives.Contains(GetCurrentPerspective()));
    }
}