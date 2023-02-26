using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using YuoTools;

[RequireComponent(typeof(LayoutGroup))]
[DisallowMultipleComponent]
public class ButtonToggleGroup : MonoBehaviour
{
    public List<ButtonToggle> toggles = new List<ButtonToggle>();
    public int showIndex;
    public bool Horizontal = true;

    [Header("?????§³???")]
    [Range(0, 1)]
    public float Size = 0.7f;

    private RectTransform rect;
    public ButtonToggle NowToggle;

    [Range(0, 0.5f)]
    public float AnimaTime;

    protected void Start()
    {
        rect = transform as RectTransform;
        rect.sizeDelta = rect.sizeDelta.RSetX(rect.sizeDelta.x * Screen.width / 2160f / Screen.height * 1080f);
        toggles.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
        showIndex.Clamp(toggles.Count - 1);
        SwitchToggle(toggles[showIndex]);
    }

    public void SwitchToggle(ButtonToggle toggle)
    {
        foreach (var item in toggles)
        {
            if (toggle == item)
            {
                NowToggle = toggle;
                item.IsOn = true;
                item.OnValueChange?.Invoke(true);
                if (Horizontal)
                {
                    item.rect.sizeDelta.x.To(rect.sizeDelta.x * Size, AnimaTime, x =>
                    {
                        item.rect.sizeDelta = item.rect.sizeDelta.RSetX(x);
                    });
                }
                else
                {
                    item.rect.sizeDelta.y.To(rect.sizeDelta.y * Size, AnimaTime, x =>
                    {
                        item.rect.sizeDelta = item.rect.sizeDelta.RSetY(x);
                    });
                }
            }
            else
            {
                item.IsOn = false;
                item.OnValueChange?.Invoke(false);
                if (Horizontal)
                {
                    item.rect.sizeDelta.x.To(rect.sizeDelta.x * (1 - Size) / (toggles.Count - 1), AnimaTime, x =>
                    {
                        item.rect.sizeDelta = item.rect.sizeDelta.RSetX(x);
                    });
                }
                else
                {
                    item.rect.sizeDelta.y.To(rect.sizeDelta.y * (1 - Size) / (toggles.Count - 1), AnimaTime, x =>
                    {
                        item.rect.sizeDelta = item.rect.sizeDelta.RSetY(x);
                    });
                }
            }
        }
        gameObject.ReShow();
    }
}