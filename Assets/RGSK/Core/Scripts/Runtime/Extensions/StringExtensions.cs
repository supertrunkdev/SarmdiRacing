using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK.Extensions
{
    public static class StringExtensions
    {
        public static string AddColorTags(this string str, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>";
        }

        public static string AddSizeTags(this string str, float size)
        {
            if (size < 0)
            {
                return str;
            }

            return $"<size={size}>{str}</size>";
        }
    }
}