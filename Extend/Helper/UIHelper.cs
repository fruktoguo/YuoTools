using UnityEngine.Events;
using UnityEngine.UI;

namespace YuoTools.Extend.Helper
{
    public static class UIHelper
    {
        public static void SetBtnClick(this Button btn, UnityAction action)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }
}