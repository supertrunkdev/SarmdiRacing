using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using RGSK.Extensions;

namespace RGSK
{
    public class SelectionItemEntry : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] Image image;
        [SerializeField] GameObject locked;
        [SerializeField] UISelectionHandler selectHandler;

        public void Setup(string text, Sprite img, Color col, bool isLocked, Action onSelect, Action onClick)
        {
            if (text != null)
            {
                this.text.text = text;
            }

            if (image != null)
            {
                image.sprite = img;
                image.color = col;
            }

            if (locked != null)
            {
                locked.SetActive(isLocked);
            }

            if (selectHandler != null)
            {
                selectHandler.onSelect.AddListener(() =>
                {
                    onSelect?.Invoke();
                });

                selectHandler.onClick.AddListener(() =>
                {
                    onClick?.Invoke();
                });
            }
        }
    }
}