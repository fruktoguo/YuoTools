using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace YuoTools.UI
{
    public class DragToggle : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public List<RectTransform> rects;

        [EnumToggleButtons] public SlideType slideType;

        [SerializeField] bool useScale = false;

        [ShowIf("useScale")] public float scale = 0.9f;

        [SerializeField] bool limiter = true;

        [ShowIf("limiter")] public float limiterPower = 0.1f;

        /// <summary>
        /// 是否使用贝塞尔曲线进行曲线运动
        /// </summary>
        [SerializeField] bool useBessel = false;

        [ShowIf("useBessel")] [BoxGroup("控制点")]
        public RectTransform start;

        [ShowIf("useBessel")] [BoxGroup("控制点")]
        public RectTransform con;

        [ShowIf("useBessel")] [BoxGroup("控制点")]
        public RectTransform end;

        [PropertyRange("scale", 1)] public float fadeLimiter = 0.7f;

        public float pos;

        public float Speed;

        public float SelectPos = 0.5f;

        [ReadOnly] [SerializeField] private int nowIndex = 0;

        public int NowIndex
        {
            get => nowIndex;
            private set
            {
                if (value != nowIndex)
                {
                    nowIndex = value;
                    onValueChanged?.Invoke(value);
                }
            }
        }

        public enum SlideType
        {
            Horizontal,
            Vertical
        }

        public UnityEvent<int> onValueChanged;

        public UnityEvent<int, float> onDragChanged;
        public UnityEvent<int> onUp;

        private void Start()
        {
            // SetItemPos(0.0f);
            dragWidth = Rect.rect.width;
            dragHeight = Rect.rect.height;
        }

        public Vector2 LastClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            LastClick = eventData.position;
        }

        private void ChangeValue(float f)
        {
            pos = f;
            SetItemPos(f);
        }

        public float dragWidth;

        public float dragHeight;

        public void OnDrag(PointerEventData eventData)
        {
            switch (slideType)
            {
                case SlideType.Horizontal:
                    ChangeValue((eventData.position - LastClick).x / dragWidth * Speed + pos);
                    break;

                case SlideType.Vertical:
                    ChangeValue((eventData.position - LastClick).y / dragHeight * Speed + pos);
                    break;

                default:
                    break;
            }

            LastClick = eventData.position;
        }

        private void OnUp()
        {
            float a = (int)((-pos + 0.5f / (rects.Count - 1)) * (rects.Count - 1)) / (float)(rects.Count - 1);
            NowIndex = (int)(a * (rects.Count - 1) + 0.5f).RClamp(0, rects.Count - 1);
            a.Clamp(1);
            pos.To(-a, 0.3f, ChangeValue, 0, () => { onUp?.Invoke(nowIndex); });
        }

        private float timer;

        public void SetItemPos(float jindu)
        {
            pos = jindu;

            if (limiter)
            {
                pos = -Mathf.Clamp(-jindu, 0 - limiterPower, 1 + limiterPower);
            }

            for (int i = 0; i < rects.Count; i++)
            {
                SetPos(i, pos + SelectPos + i * 1f / (rects.Count - 1));
            }
        }

        public void SetItemPosOfIndex(int index)
        {
            NowIndex = index;
            pos = -index * (1f / (rects.Count - 1));
            SetItemPos(pos);
        }

        RectTransform _rectTransform;

        public RectTransform Rect
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void SetPos(int index, float ro)
        {
            var rect = rects[index];


            float abs = 1 - Mathf.Abs(ro - SelectPos);
            abs.Clamp(scale, 1);
            if (useScale)
            {
                rect.localScale = Vector3.one * abs;
            }

            if (limiter)
                onDragChanged?.Invoke(index, abs.RemapClamp(fadeLimiter, 1, 0, 1));
            else
                onDragChanged?.Invoke(index, abs.RemapClamp(scale, 1, 0, 1));

            if (useBessel)
            {
                rect.transform.position = YuoTool.CalculateCubicBezierPoint2D(ro, start.transform.position,
                    con.transform.position, end.transform.position);
                rect.anchoredPosition3D = rect.anchoredPosition3D.RSetZ(0);
            }
            else
            {
                rect.SetAnchoredPosX(ro.Remap(0, 1, 0, Rect.rect.size.x));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUp();
        }
    }
}