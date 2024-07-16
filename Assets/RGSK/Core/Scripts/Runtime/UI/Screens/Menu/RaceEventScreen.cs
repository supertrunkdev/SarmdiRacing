using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RGSK.Extensions;

namespace RGSK
{
    public class RaceEventScreen : UIScreen
    {
        [SerializeField] List<RaceSession> eventDefinitions = new List<RaceSession>();
        [SerializeField] RaceEventEntry entryPrefab;
        [SerializeField] ScrollRect scrollView;

        List<RaceEventEntry> _entries = new List<RaceEventEntry>();

        public override void Initialize()
        {
            if (entryPrefab != null && scrollView != null)
            {
                foreach (var e in eventDefinitions)
                {
                    var entry = Instantiate(entryPrefab, scrollView.content);
                    entry.Setup(e);
                    _entries.Add(entry);
                }
            }

            base.Initialize();
        }

        public override void Open()
        {
            base.Open();

            foreach (var e in _entries)
            {
                e.Refresh();
            }

            StartCoroutine(scrollView?.SelectChild(0));
        }
    }
}