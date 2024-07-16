using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class TabController : MonoBehaviour
    {
        List<TabButton> _buttons = new List<TabButton>();
        TabButton _selected;
        UIScreen _screen;

        void OnEnable()
        {
            InputManager.TabChangedEvent += ChangeTab;
        }

        void OnDisable()
        {
            InputManager.TabChangedEvent -= ChangeTab;
        }

        void Awake()
        {
            _screen = GetComponentInParent<UIScreen>();

            foreach (var btn in GetComponentsInChildren<TabButton>())
            {
                RegisterTabButton(btn);
                btn.Setup(this);
            }
        }

        public void RegisterTabButton(TabButton button)
        {
            if (!_buttons.Contains(button))
            {
                _buttons.Add(button);
            }

            _buttons = _buttons.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        }

        public void UnregisterTabButton(TabButton button)
        {
            if (_buttons.Contains(button))
            {
                _buttons.Remove(button);
                ChangeTab(0);
            }

            _buttons = _buttons.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        }

        public void SelectTab(TabButton button)
        {
            _selected = button;

            foreach (var btn in _buttons)
            {
                btn.ToggleActive(btn == _selected);
            }
        }

        public void ChangeTab(int direction)
        {
            if (_screen != null && !_screen.IsOpen() || ModalWindowManager.Instance.IsOpen())
                return;

            var index = 0;
            if (_selected != null)
            {
                index = _buttons.IndexOf(_selected);
            }

            index += direction;
            index = Mathf.Clamp(index, 0, _buttons.Count - 1);

            _buttons[index].OnSubmit(null);
        }
    }
}