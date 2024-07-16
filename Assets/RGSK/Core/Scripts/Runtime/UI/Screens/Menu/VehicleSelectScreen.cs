using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class VehicleSelectScreen : UIScreen
    {
        [Header("Vehicle Select")]
        [SerializeField] SelectionItemEntry entryPrefab;
        [SerializeField] ScrollRect vehicleScrollView;
        [SerializeField] VehicleDefinitionUI vehicleDefinitionUI;
        [SerializeField] Button selectButton;
        [SerializeField] bool createLockedItemEntries = true;
        [SerializeField] bool destroyVehicleOnClose = true;

        [Header("Color Select")]
        [SerializeField] SelectionItemEntry colorEntryPrefab;
        [SerializeField] ScrollRect colorScrollView;

        public UnityEvent OnSelected;

        MenuVehicleSpawner _vehicleSpawner;
        VehicleDefinition _selected;
        bool _inColorSelect;

        public override void Initialize()
        {
            if (vehicleScrollView != null && entryPrefab != null)
            {
                foreach (var v in RGSKCore.Instance.ContentSettings.vehicles)
                {
                    if(!createLockedItemEntries && !v.IsUnlocked())
                        continue;
                    
                    if (v != null)
                    {
                        var e = Instantiate(entryPrefab, vehicleScrollView.content);
                        e.Setup(
                        v.objectName,
                        v.previewPhoto,
                        Color.white,
                        !v.IsUnlocked(),
                        () =>
                        {
                            if (!GeneralHelper.IsMobilePlatform())
                            {
                                UpdateVehicle(v);
                            }
                        },
                        () =>
                        {
                            if (GeneralHelper.IsMobilePlatform())
                            {
                                UpdateVehicle(v);
                            }
                            else
                            {
                                Select();
                            }
                        });
                    }
                }
            }

            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() => Select());
            }

            base.Initialize();
        }

        public override void Open()
        {
            base.Open();
            ToggleColorSelection(false);
        }

        public override void Close()
        {
            base.Close();
            _selected = null;

            if (destroyVehicleOnClose)
            {
                _vehicleSpawner?.DestroyActiveVehicle();
            }
        }

        public override void Back()
        {
            if (!_inColorSelect)
            {
                base.Back();
                return;
            }

            ToggleColorSelection(false);
        }

        void Select()
        {
            if (!_selected.IsUnlocked())
                return;

            if (_inColorSelect)
            {
                OnSelected.Invoke();
                return;
            }

            RGSKCore.runtimeData.SelectedVehicle = _selected;
            SaveData.Instance.playerData.selectedVehicleID = _selected.ID;

            if (CanSelectColors())
            {
                PopulateColorSelection();
                ToggleColorSelection(true);
            }
            else if (CanSelectLiveries())
            {
                PopulateLiverySelection();
                ToggleColorSelection(true);
            }
            else
            {
                OnSelected.Invoke();
            }
        }

        void UpdateVehicle(VehicleDefinition definition)
        {
            if (_selected == definition)
                return;

            if (_vehicleSpawner == null)
            {
                _vehicleSpawner = FindObjectOfType<MenuVehicleSpawner>();
            }

            _selected = definition;
            vehicleDefinitionUI?.UpdateUI(_selected);
            _vehicleSpawner?.Spawn(_selected);
        }

        void ToggleColorSelection(bool toggle)
        {
            if (vehicleScrollView == null || colorScrollView == null)
                return;

            vehicleScrollView.gameObject.SetActive(!toggle);
            colorScrollView.gameObject.SetActive(toggle);

            if (toggle)
            {
                var index = 0;

                if (_vehicleSpawner?.VehicleInstance() != null)
                {
                    index = GeneralHelper.GetVehicleColorIndex(_vehicleSpawner.VehicleInstance(), 0);
                }

                StartCoroutine(colorScrollView.SelectChild(index));
            }
            else
            {
                var index = 0;

                if (RGSKCore.runtimeData.SelectedVehicle != null)
                {
                    index = RGSKCore.Instance.ContentSettings.vehicles.IndexOf(RGSKCore.runtimeData.SelectedVehicle);

                    if (index < 0)
                    {
                        index = 0;
                    }
                }

                UpdateVehicle(RGSKCore.Instance.ContentSettings.vehicles[index]);
                StartCoroutine(vehicleScrollView.SelectChild(index));
                ClearColorScrollView();
            }

            _inColorSelect = toggle;
        }

        void PopulateColorSelection()
        {
            var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;

            if (colorScrollView == null || colorEntryPrefab == null || colors == null)
                return;

            foreach (var c in colors.colors)
            {
                var e = Instantiate(colorEntryPrefab, colorScrollView.content);

                e.Setup(
                "",
                null,
                c,
                false,
                () =>
                {
                    if (!GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateColor(c);
                    }
                },
                () =>
                {
                    if (GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateColor(c);
                    }
                    else
                    {
                        Select();
                    }
                });
            }
        }

        void PopulateLiverySelection()
        {
            var liveries = GeneralHelper.GetVehicleLiveries(_vehicleSpawner.VehicleInstance());

            if (colorScrollView == null || colorEntryPrefab == null || liveries == null)
                return;

            foreach (var l in liveries.liveries)
            {
                var e = Instantiate(colorEntryPrefab, colorScrollView.content);

                e.Setup(
                "",
                l.preview,
                l.preview == null ? l.previewColor : Color.white,
                false,
                () =>
                {
                    if (!GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateLivery(l.texture);
                    }
                },
                () =>
                {
                    if (GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateLivery(l.texture);
                    }
                    else
                    {
                        Select();
                    }
                });
            }
        }

        void ClearColorScrollView()
        {
            if (colorScrollView == null)
                return;

            colorScrollView.content.gameObject.DestroyAllChildren();
        }

        bool CanSelectColors()
        {
            if (_vehicleSpawner?.VehicleInstance() != null)
            {
                var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;
                return GeneralHelper.CanApplyColor(_vehicleSpawner.VehicleInstance()) && colors != null && colors.colors.Count > 0;
            }

            return false;
        }

        bool CanSelectLiveries()
        {
            if (_vehicleSpawner?.VehicleInstance() != null)
            {
                var liveries = GeneralHelper.GetVehicleLiveries(_vehicleSpawner.VehicleInstance());
                return liveries != null && liveries.liveries.Count > 0;
            }

            return false;
        }
    }
}