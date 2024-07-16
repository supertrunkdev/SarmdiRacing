using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using RGSK.Helpers;

namespace RGSK
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer Mixer => RGSKCore.Instance.AudioSettings.mixer;

        SoundList sounds => RGSKCore.Instance.AudioSettings.sounds;

        public void Play(string sound, AudioGroup group, Vector3? position = null)
        {
            if (string.IsNullOrWhiteSpace(sound))
                return;

            Play(sound, group.ToString(), position);
        }

        void Play(string key, string group, Vector3? position)
        {
            if (sounds == null)
            {
                Logger.Log("Sounds have not been assigned!");
                return;
            }

            var clip = sounds.GetSound(key);

            if (clip != null)
            {
                var src = AudioHelper.CreateAudioSource(
                                                        clip,
                                                        !position.HasValue,
                                                        false,
                                                        true,
                                                        1,
                                                        1,
                                                        group,
                                                        null,
                                                        key);

                if (position.HasValue)
                {
                    src.transform.position = position.Value;
                    src.rolloffMode = AudioRolloffMode.Linear;
                    src.maxDistance = 50;
                    src.dopplerLevel = 0;
                }

                src.gameObject.AddComponent<AutoDestroyAudiosource>();
            }
        }
    }
}