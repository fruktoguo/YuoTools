using System.Collections.Generic;
using UnityEngine;

namespace YuoTools.Extend.UI
{
    [CreateAssetMenu(fileName = "YuoLanguageTemplate", menuName = "YuoLanguage/创建设置模板", order = 0)]
    public class YuoLanguageTemplate : ScriptableObject
    {
        public List<string> Text = new();
        public List<string> Image = new();
        public List<string> Sound = new();
        public List<string> Video = new();
    }
}