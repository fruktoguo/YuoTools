using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using YuoTools;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class SwitchToggle : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public List<RectTransform> rects;

    [EnumToggleButtons]
    public SlideType slideType;

    [BoxGroup("控制点")]
    public RectTransform start;

    [BoxGroup("控制点")]
    public RectTransform con;

    [BoxGroup("控制点")]
    public RectTransform end;

    public float pos;
    public float Speed;

    [ReadOnly]
    [SerializeField]
    private int nowIndex = 0;

    public int NowIndex
    {
        get => nowIndex;
        private set
        {
            if (value != nowIndex)
            {
                nowIndex = value;
                OnValueChange?.Invoke(value);
            }
        }
    }

    public enum SlideType
    {
        Horizontal,
        Vertical
    }

    public UnityEvent<int> OnValueChange;

    private void Start()
    {
        SetItemPos(0.0f);
    }

    public Vector2 LastClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        LastClick = eventData.position;
    }

    public void ChangeValue(float f)
    {
        pos = f;
        SetItemPos(f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (slideType)
        {
            case SlideType.Horizontal:
                ChangeValue((eventData.position - LastClick).x / 1920 * -Speed + pos);
                break;

            case SlideType.Vertical:
                ChangeValue((eventData.position - LastClick).y / 1080 * -Speed + pos);
                break;

            default:
                break;
        }
        LastClick = eventData.position;
    }

    public void OnUp()
    {
        float a = (int)((pos + 0.5f / (rects.Count - 1)) * (rects.Count - 1)) / (float)(rects.Count - 1);
        NowIndex = (int)(a * (rects.Count - 1) + 0.5f);
        a.Clamp(1);
        pos.To(a, 0.3f, x =>
        {
            //uuuu(x);
            ChangeValue(x);
        }, 0, () =>
         {
         }
        );
    }

    private float timer;

    public void SetItemPos(float jindu)
    {
        pos = jindu;
        for (int i = 0; i < rects.Count; i++)
        {
            SetPos(rects[i], jindu - 0.5f + i * 1f / (rects.Count - 1));
        }
    }

    public void SetPos(RectTransform rect, float ro)
    {
        float scale = 0.2f - Mathf.Abs(ro - 0.5f);
        scale.Clamp(0, 0.2f);
        rect.localScale = Vector3.one * (1 + scale * 5);
        //rect.anchoredPosition = YuoTool.CalculateCubicBezierPoint2D(ro, start.anchoredPosition, con.anchoredPosition, end.anchoredPosition);
        rect.transform.position = YuoTool.CalculateCubicBezierPoint2D(ro, start.transform.position, con.transform.position, end.transform.position);
        rect.anchoredPosition3D = rect.anchoredPosition3D.RSetZ(0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp();
    }
}