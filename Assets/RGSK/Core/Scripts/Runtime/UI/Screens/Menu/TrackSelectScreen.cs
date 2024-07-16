using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class TrackSelectScreen : UIScreen
    {
        [SerializeField] SelectionItemEntry entryPrefab;
        [SerializeField] ScrollRect scrollView;
        [SerializeField] TrackDefinitionUI trackDefinitionUI;
        [SerializeField] Button selectButton;
        [SerializeField] bool createLockedItemEntries = true;

        public UnityEvent OnSelected;

        TrackDefinition _selected;

        public override void Initialize()
        {
            if (scrollView != null)
            {
                foreach (var t in RGSKCore.Instance.ContentSettings.tracks)
                {
                    if(!createLockedItemEntries && !t.IsUnlocked())
                        continue;

                    if (t != null)
                    {
                        var e = Instantiate(entryPrefab, scrollView.content);
                        e.Setup(
                        t.objectName,
                        t.previewPhoto,
                        Color.white,
                        !t.IsUnlocked(),
                        () =>
                        {
                            if (!GeneralHelper.IsMobilePlatform())
                            {
                                UpdateTrack(t);
                            }
                        },
                        () =>
                        {
                            if (GeneralHelper.IsMobilePlatform())
                            {
                                UpdateTrack(t);
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

            if (RGSKCore.Instance.ContentSettings.tracks.Count > 0)
            {
                var index = 0;

                if (RGSKCore.runtimeData.SelectedTrack != null)
                {
                    index = RGSKCore.Instance.ContentSettings.tracks.IndexOf(RGSKCore.runtimeData.SelectedTrack);

                    if (index < 0)
                    {
                        index = 0;
                    }
                }

                UpdateTrack(RGSKCore.Instance.ContentSettings.tracks[index]);
                StartCoroutine(scrollView?.SelectChild(index));
            }
        }

        public override void Close()
        {
            base.Close();
            _selected = null;
        }

        void UpdateTrack(TrackDefinition definition)
        {
            if (_selected == definition)
                return;

            _selected = definition;
            trackDefinitionUI?.UpdateUI(_selected);
        }

        void Select()
        {
            if (!_selected.IsUnlocked())
                return;

            RGSKCore.runtimeData.SelectedTrack = _selected;
            OnSelected.Invoke();
        }
    }
}