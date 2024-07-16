using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceEventEntry : MonoBehaviour
    {
        [SerializeField] RaceSession raceEvent;
        [SerializeField] Image photo;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text bestPositionText;
        [SerializeField] Image trophy;
        [SerializeField] GameObject locked;

        [Header("Modal")]
        [SerializeField] ModalWindowProperties modalProperties;
        [SerializeField] bool showModalWindow;

        RaceSession _event;

        void Start()
        {
            if (raceEvent != null)
            {
                Setup(raceEvent);
            }
        }

        public void Setup(RaceSession raceEvent)
        {
            _event = raceEvent;

            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => Select());
            }

            Refresh();
        }

        void Select()
        {
            if (!_event.IsUnlocked())
                return;

            if (showModalWindow)
            {
                ModalWindowManager.Instance.Show(new ModalWindowProperties
                {
                    header = modalProperties.header,
                    message = modalProperties.message,
                    confirmButtonText = modalProperties.confirmButtonText,
                    declineButtonText = modalProperties.declineButtonText,
                    confirmAction = () => Load(),
                    declineAction = () => { },
                    startSelection = modalProperties.startSelection
                });
            }
            else
            {
                Load();
            }
        }

        public void Refresh()
        {
            if (photo != null)
            {
                photo.sprite = _event.previewPhoto;
            }

            if (icon != null)
            {
                icon.sprite = _event.icon;
                icon.DisableIfNullSprite();
            }

            if (locked != null)
            {
                locked.SetActive(!_event.IsUnlocked());
            }

            nameText?.SetText(_event.objectName);
            descriptionText?.SetText(_event.description);

            var record = _event != null ? _event.LoadBestPosition() : -1;
            bestPositionText?.SetText(record > 0 ? UIHelper.FormatOrdinalText(record) : "");

            if (trophy != null)
            {
                var icon = TargetScoreIcon.GetIcon(record - 1);
                trophy.enabled = icon != null;

                if (icon != null)
                {
                    trophy.sprite = icon.icon;
                    trophy.color = icon.color;
                }
            }
        }

        void Load()
        {
            RGSKCore.runtimeData.SelectedSession = _event;
            RGSKCore.runtimeData.SelectedTrack = _event.track;

            if (_event?.track?.scene != null)
            {
                SceneLoadManager.LoadScene(_event.track.scene);
            }
        }
    }
}