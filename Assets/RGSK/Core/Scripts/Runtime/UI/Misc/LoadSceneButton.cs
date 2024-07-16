using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] SceneReference scene;
        [SerializeField] ModalWindowProperties modalProperties;
        [SerializeField] bool showModalWindow;

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                if (showModalWindow)
                {
                    btn.onClick.AddListener(() =>
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
                    });
                }
                else
                {
                    btn.onClick.AddListener(() => Load());
                }
            }
        }

        void Load() => SceneLoadManager.LoadScene(scene);
    }
}