using UnityEngine.UI;
using YuoTools.UI;

namespace YuoTools.Extend.UI
{
    public partial class SpawnUICodeConfig
    {
        public void Init1()
        {
            SpawnType.Add(typeof(YuoUIDrag));
            SpawnType.Add(typeof(ButtonSwitch));
            SpawnType.Add(typeof(ButtonSwitchGo));
            SpawnType.Add(typeof(YuoDropDown));
            SpawnType.Add(typeof(YuoToggle));
            SpawnType.Add(typeof(YuoToggleGroup));
            SpawnType.Add(typeof(DragToggle));
            
            RemoveType.Add(typeof(YuoDropDown), typeof(Button));
            RemoveType.Add(typeof(ButtonSwitch), typeof(Button));
            RemoveType.Add(typeof(ButtonSwitchGo), typeof(Button));
        }
    }
}