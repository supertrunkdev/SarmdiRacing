using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class SharedButton : MonoBehaviour
    {
        [SerializeField] ButtonType type;
        [SerializeField] Button button;

        void Start()
        {
            switch (type)
            {
                case ButtonType.Restart:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = "Restart",
                            message = "Are you sure you want to restart?",
                            confirmButtonText = "Yes",
                            declineButtonText = "No",
                            confirmAction = Restart,
                            declineAction = () => { },
                            startSelection = 1
                        }));

                        break;
                    }

                case ButtonType.BackToMenu:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = "Exit",
                            message = "Are you sure you want to exit?",
                            confirmButtonText = "Yes",
                            declineButtonText = "No",
                            confirmAction = () => SceneLoadManager.LoadMainScene(),
                            declineAction = () => { },
                            startSelection = 1
                        }));
                        break;
                    }

                case ButtonType.QuitApplication:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = "Quit",
                            message = "Are you sure you want to quit?",
                            confirmButtonText = "Yes",
                            declineButtonText = "No",
                            confirmAction = () => SceneLoadManager.QuitApplication(),
                            declineAction = () => { },
                            startSelection = 1
                        }));
                        break;
                    }

                case ButtonType.WatchReplay:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (RaceManager.Instance.Initialized &&
                                RaceManager.Instance.CurrentState != RaceState.PostRace)
                            {
                                return;
                            }

                            RecorderManager.Instance?.ReplayRecorder?.StartPlayback();
                        });
                        break;
                    }
            }
        }

        void Restart()
        {
            PauseManager.Instance.Unpause();

            if (RaceManager.Instance.Initialized)
            {
                RaceManager.Instance.RestartRace();
                return;
            }

            SceneLoadManager.ReloadScene();
        }
    }
}