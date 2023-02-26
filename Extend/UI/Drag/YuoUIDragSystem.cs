using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public class YuoUIDragManager : YuoComponentInstance<YuoUIDragManager>
    {
        YuoUIDrag dragItem;

        public YuoUIDrag DragItem
        {
            get => dragItem;
            set
            {
                if (dragItem != null) dragItem.graphic.raycastTarget = true;
                if (value != null) value.graphic.raycastTarget = false;
                dragItem = value;
            }
        }
    }

    public class YuoUIDragSystem : YuoSystem<YuoUIDragManager>, IUpdate
    {
        protected override void Run(YuoUIDragManager component)
        {
            if (component.DragItem != null)
            {
                //����ק�����ƶ����������λ��
                component.DragItem.graphic.raycastTarget = false;
                component.DragItem.transform.position = Input.mousePosition;
            }
        }
    }
}