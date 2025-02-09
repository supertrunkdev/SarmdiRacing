using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

namespace RGSK
{
    public class TMPTextSwitcherMixerBehaviour : PlayableBehaviour
    {
        Color m_DefaultColor;
        float m_DefaultFontSize;
        string m_DefaultText;

        TMP_Text m_TrackBinding;
        bool m_FirstFrameHappened;

        public override void ProcessFrame(Playable playable, UnityEngine.Playables.FrameData info, object playerData)
        {
            m_TrackBinding = playerData as TMP_Text;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened)
            {
                m_DefaultColor = m_TrackBinding.color;
                m_DefaultFontSize = m_TrackBinding.fontSize;
                m_DefaultText = m_TrackBinding.text;
                m_FirstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            float blendedFontSize = 0f;
            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TMPTextSwitcherBehaviour> inputPlayable = (ScriptPlayable<TMPTextSwitcherBehaviour>)playable.GetInput(i);
                TMPTextSwitcherBehaviour input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                blendedFontSize += input.fontSize * inputWeight;
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    m_TrackBinding.text = input.text;
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
            m_TrackBinding.fontSize = Mathf.RoundToInt(blendedFontSize + m_DefaultFontSize * (1f - totalWeight));
            if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
            {
                m_TrackBinding.text = m_DefaultText;
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            if (m_TrackBinding == null)
                return;

            m_TrackBinding.color = m_DefaultColor;
            m_TrackBinding.fontSize = m_DefaultFontSize;
            m_TrackBinding.text = m_DefaultText;
            m_FirstFrameHappened = false;
        }
    }
}