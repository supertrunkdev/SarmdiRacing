using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RGSK.Helpers;

namespace RGSK
{
    public class GameSettingsGamplayEntryUI : GameSettingsEntryUI //Gameplay* pardon the typo
    {
        [SerializeField] GameplaySettingType type;

        protected override void Start()
        {
            base.Start();

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(type.ToString());
            }
        }

        public override void SelectOption(int direction)
        {
            switch (type)
            {
                case GameplaySettingType.SpeedUnit:
                    {
                        SetSpeedUnit(direction);
                        break;
                    }

                case GameplaySettingType.DistanceUnit:
                    {
                        SetDistanceUnit(direction);
                        break;
                    }

                case GameplaySettingType.Transmission:
                    {
                        SetTransmission(direction);
                        break;
                    }

                case GameplaySettingType.Nameplate:
                    {
                        SetNameplate(direction);
                        break;
                    }

                case GameplaySettingType.MobileInput:
                    {
                        SetMobileInput(direction);
                        break;
                    }

                case GameplaySettingType.Vibration:
                    {
                        SetVibration(direction);
                        break;
                    }

                case GameplaySettingType.FPS:
                    {
                        SetFPS(direction);
                        break;
                    }
            }
        }

        void SetSpeedUnit(int direction)
        {
            var count = Enum.GetValues(typeof(SpeedUnit)).Length;
            var index = (int)RGSKCore.Instance.UISettings.speedUnit;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.UISettings.speedUnit = (SpeedUnit)index;
            valueText?.SetText(((SpeedUnit)index).ToString());
        }

        void SetDistanceUnit(int direction)
        {
            var count = Enum.GetValues(typeof(DistanceUnit)).Length;
            var index = (int)RGSKCore.Instance.UISettings.distanceUnit;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.UISettings.distanceUnit = (DistanceUnit)index;
            valueText?.SetText(((DistanceUnit)index).ToString());
        }

        void SetTransmission(int direction)
        {
            var count = Enum.GetValues(typeof(TransmissionType)).Length;
            var index = (int)RGSKCore.Instance.VehicleSettings.transmissionType;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.VehicleSettings.transmissionType = (TransmissionType)index;
            valueText?.SetText(((TransmissionType)index).ToString());
        }

        void SetNameplate(int direction)
        {
            var index = RGSKCore.Instance.UISettings.showNameplates ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.UISettings.showNameplates = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetMobileInput(int direction)
        {
            var count = Enum.GetValues(typeof(MobileControlType)).Length;
            var index = (int)RGSKCore.Instance.InputSettings.mobileControlType;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.InputSettings.mobileControlType = (MobileControlType)index;
            valueText?.SetText(((MobileControlType)index).ToString());
        }

        void SetVibration(int direction)
        {
            var index = RGSKCore.Instance.InputSettings.vibrate ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.InputSettings.vibrate = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetFPS(int direction)
        {
            var index = RGSKCore.Instance.GeneralSettings.enableFpsReader ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.GeneralSettings.enableFpsReader = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }
    }
}