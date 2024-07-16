using System;
using UnityEngine;
using RGSK.Extensions;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RGSK.Helpers
{
    public static class GeneralHelper
    {
        static Dictionary<float, WaitForSeconds> _cachedWFS = new Dictionary<float, WaitForSeconds>();
        static Dictionary<float, WaitForSecondsRealtime> _cachedWFSRT = new Dictionary<float, WaitForSecondsRealtime>();
        static Transform _dynamicParent;

        #region Entity
        public static RGSKEntity GetFocusedEntity()
        {
            return CameraManager.Instance?.FocusedEntity ?? null;
        }

        public static RGSKEntity GetEntity(int index)
        {
            return RGSKCore.Instance.GeneralSettings.entitySet.GetItem(index) ?? null;
        }

        public static int GetEntityIndex(GameObject go)
        {
            return RGSKCore.Instance.GeneralSettings.entitySet.GetIndexOf(go);
        }
        #endregion

        #region Race
        public static int GetLapsAhead(Competitor a, Competitor b)
        {
            var lapDist = a.TotalRaceDistance / a.TotalLaps;
            var gap = GetDistanceGapBetween(a, b);

            return (int)Mathf.Abs(gap / lapDist);
        }

        public static float GetDistanceGapBetween(Competitor a, Competitor b)
        {
            if (a == null || b == null)
                return 0;

            return a.DistanceTravelled - b.DistanceTravelled;
        }

        public static float GetTimeGapBetween(Competitor a, Competitor b)
        {
            if (a == null || b == null)
                return 0;

            if (b.Entity.CurrentSpeed < 1)
            {
                return 0;
            }

            var dist = GetDistanceGapBetween(a, b);
            return dist / Math.Abs(b.Entity.CurrentSpeed);
        }
        #endregion

        #region Misc
        public static void SetTransmission(GameObject go, TransmissionType type)
        {
            if (go.TryGetComponent<IVehicle>(out var v))
            {
                v.TransmissionType = type;
            }
        }

        public static void SetHandlingMode(GameObject go, VehicleHandlingMode mode)
        {
            if (go.TryGetComponent<IVehicle>(out var v))
            {
                v.HandlingMode = mode;
            }
        }

        public static void ToggleGhostedMesh(GameObject target, bool on)
        {
            var materialController = target.GetOrAddComponent<GhostMaterialController>();
            materialController.ToggleGhostMaterials(on);
        }

        public static void SetColor(GameObject target, Color col, int colIndex = 0)
        {
            var colors = target.GetComponentsInChildren<MeshColorizer>();
            if (colors.Length > 0)
            {
                foreach (var c in colors)
                {
                    c.SetColor(col, colIndex);
                }
            }
        }

        public static void SetColor(GameObject target, int col, int colIndex = 0)
        {
            var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;
            if (colors == null)
                return;

            col = Mathf.Clamp(col, 0, colors.colors.Count - 1);
            SetColor(target, colors.colors[col]);
        }

        public static Color GetRandomVehicleColor() => RGSKCore.Instance.VehicleSettings.vehicleColorList?.GetRandom() ?? Color.white;

        public static Color GetColor(GameObject target, int colIndex = 0)
        {
            var color = target.GetComponentsInChildren<MeshColorizer>().FirstOrDefault(x => x.colorIndex == colIndex);

            if (color != null)
            {
                return color.GetColor();
            }

            return Color.black;
        }

        public static int GetVehicleColorIndex(GameObject target, int colIndex = 0)
        {
            var vehicleColor = GetColor(target, colIndex);

            if (RGSKCore.Instance.VehicleSettings.vehicleColorList != null)
            {
                for (int i = 0; i < RGSKCore.Instance.VehicleSettings.vehicleColorList.colors.Count; i++)
                {
                    var color = RGSKCore.Instance.VehicleSettings.vehicleColorList.colors[i];

                    if (ColorUtility.ToHtmlStringRGBA(vehicleColor) == ColorUtility.ToHtmlStringRGBA(color))
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        public static void SetLivery(GameObject target, Texture2D tex)
        {
            var liveries = target.GetComponentsInChildren<MeshLivery>();

            if (liveries.Length > 0)
            {
                foreach (var l in liveries)
                {
                    l.SetLivery(tex);
                }
            }
        }

        public static void SetLivery(GameObject target, int texIndex)
        {
            var liveries = GetVehicleLiveries(target);

            if (liveries != null)
            {
                texIndex = Mathf.Clamp(texIndex, 0, liveries.liveries.Count - 1);
                SetLivery(target, liveries.liveries[texIndex].texture);
            }
        }

        public static void SetRandomLivery(GameObject target)
        {
            var liveries = GetVehicleLiveries(target);

            if (liveries != null)
            {
                SetLivery(target, liveries.GetRandom().texture);
            }
        }

        public static int GetLiveryIndex(GameObject target)
        {
            var livery = target.GetComponentInChildren<MeshLivery>();

            if (livery != null)
            {
                return livery.CurrentLiveryIndex;
            }

            return 0;
        }

        public static LiveryList GetVehicleLiveries(GameObject target)
        {
            var livery = target.GetComponentInChildren<MeshLivery>();

            if (livery != null)
            {
                return livery.Liveries;
            }

            return null;
        }

        public static bool CanApplyColor(GameObject target)
        {
            return target.GetComponentInChildren<MeshColorizer>() != null;
        }

        public static bool CanApplyLivery(GameObject target)
        {
            return target.GetComponentInChildren<MeshLivery>() != null;
        }

        public static int GetLevelFromXP(int xp)
        {
            var level = 1;

            for (int i = 1; i < RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.GetMaxTime() + 1; i++)
            {
                if (xp > (int)RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.Evaluate(i))
                {
                    level = i;
                }
            }

            return level;
        }
        #endregion

        #region Input
        public static void TogglePlayerInput(GameObject go, bool enable)
        {
            if (enable)
            {
                PlayerVehicleInput.Instance?.Bind(go);
            }
            else
            {
                PlayerVehicleInput.Instance?.Unbind(go);
            }
        }

        public static void ToggleAIInput(GameObject go, bool enable)
        {
            var ai = go.GetOrAddComponent<AIController>();
            ai.ToggleActive(enable);

            if (enable)
            {
                SetTransmission(go, TransmissionType.Automatic);
            }
        }

        public static void ToggleInputControl(GameObject go, bool enable)
        {
            var input = go.GetComponent<IInput>();

            if (enable)
            {
                input?.EnableControl();
            }
            else
            {
                input?.DisableControl();
            }
        }
        #endregion

        #region Physics
        public static void ToggleVehicleCollision(GameObject go, bool on)
        {
            go.SetColliderLayer(on ?
                            RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index :
                            RGSKCore.Instance.GeneralSettings.ghostLayerIndex.Index);
        }

        public static void TogglePhysics(GameObject go, bool enable)
        {
            if (go.TryGetComponent<Rigidbody>(out var rigid))
            {
                rigid.isKinematic = !enable;
            }
        }

        public static void SetRigidbodySpeed(GameObject go, float speed)
        {
            if (go.TryGetComponent<Rigidbody>(out var rigid))
            {
                rigid.SetSpeed(speed, SpeedUnit.KMH);
            }
        }
        #endregion

        #region AI
        public static void SetAIBehaviour(GameObject go, AIBehaviourSettings behaviour)
        {
            if (behaviour == null)
                return;

            var ai = go.GetComponent<AIController>();

            if (ai != null)
            {
                ai.SetBehaviour(behaviour);
            }
        }
        #endregion

        #region Util
        public static Timer CreateTimer(float startValue, bool countdown, bool autoReset,
                        System.Action onElapsed = null,
                        System.Action onStart = null,
                        System.Action onStop = null,
                        System.Action onRestart = null,
                        string name = "timer",
                        bool fixedUpdate = false)
        {
            var timer = new GameObject(name).AddComponent<Timer>();

            timer.Initialize(startValue, countdown, autoReset, fixedUpdate);
            timer.OnTimerElapsed += onElapsed;
            timer.OnTimeStart += onStart;
            timer.OnTimerStop += onStop;
            timer.OnTimerRestart += onRestart;

            timer.transform.SetParent(GetDynamicParent());
            return timer;
        }

        public static Transform GetDynamicParent()
        {
            if (_dynamicParent == null)
            {
                _dynamicParent = new GameObject("[RGSK] Runtime Objects").transform;
                _dynamicParent.transform.SetAsLastSibling();
            }

            return _dynamicParent;
        }

        public static WaitForSeconds GetCachedWaitForSeconds(float value)
        {
            if (_cachedWFS.TryGetValue(value, out var wait))
            {
                return wait;
            }
            else
            {
                _cachedWFS[value] = new WaitForSeconds(value);
                return _cachedWFS[value];
            }
        }

        public static WaitForSecondsRealtime GetCachedWaitForSecondsRealtime(float value)
        {
            if (_cachedWFSRT.TryGetValue(value, out var wait))
            {
                return wait;
            }
            else
            {
                _cachedWFSRT[value] = new WaitForSecondsRealtime(value);
                return _cachedWFSRT[value];
            }
        }

        public static List<Resolution> GetResolutionsList(int minHeight = 720)
        {
            var resolutions = Screen.resolutions.Select(resolution => new Resolution
            { width = resolution.width, height = resolution.height }).Distinct();

            return resolutions.Where(x => x.height >= minHeight).ToList();
        }

        public static int GetCurrentResolutionIndex()
        {
            var resolutions = GetResolutionsList();
            var currentResolution = Screen.currentResolution;
            return resolutions.FindIndex(x => x.width == currentResolution.width && x.height == currentResolution.height);
        }

        public static int ValidateIndex(int index, int minLength, int maxLength, bool loop)
        {
            if (loop)
            {
                return (int)CustomRepeat(index, minLength, maxLength);
            }

            float CustomRepeat(float value, float minValue, float maxValue)
            {
                var range = maxValue - minValue;
                return minValue + ((value - minValue) % range + range) % range;
            }

            return Mathf.Clamp(index, minLength, maxLength);
        }

        public static ProfileDefinition CreateProfileDefinition(string name)
        {
            var names = name.Split(' ');
            var profile = ScriptableObject.CreateInstance<ProfileDefinition>();

            if (names.Length > 0)
            {
                profile.firstName = names[0];
                profile.lastName = names.Length > 1 ? names[1] : string.Empty;
            }

            return profile;
        }

        public static bool IsMobilePlatform()
        {
            if (Application.isEditor)
            {
                return false;
            }

            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
        }
        #endregion

        #region IO
        public static string GetSaveDataDirectory()
        {
            var path = Path.Combine(Application.persistentDataPath, "SaveData");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetProfilesDirectory()
        {
            var path = Path.Combine(GetSaveDataDirectory(), "Profiles");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetReplayDirectory()
        {
            var path = Path.Combine(GetSaveDataDirectory(), "Replays");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
        #endregion
    }
}