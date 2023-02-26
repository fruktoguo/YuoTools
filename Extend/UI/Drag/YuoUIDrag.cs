using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class YuoUIDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        private static YuoUIDrag _current;
        [HideInInspector]
        public MaskableGraphic graphic;
        public UnityEvent<YuoUIDrag> onDragEnd;
        public UnityEvent onDragStart;

        private void Awake()
        {
            graphic = GetComponent<MaskableGraphic>();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //print($"��ק������{gameObject.name},��ק���� {(Current != null ? Current.name : null)}");
            if(YuoUIDragManager.Instance.DragItem == this) YuoUIDragManager.Instance.DragItem = null;
            onDragEnd?.Invoke(_current);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDragStart?.Invoke();
            YuoUIDragManager.Instance.DragItem = this;
            print($"��ʼ��ק {gameObject.name}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _current = this;
            //print(gameObject.name + " OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_current == this) _current = null;
        }
    }


}