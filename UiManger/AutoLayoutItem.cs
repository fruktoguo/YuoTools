using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
[System.Serializable]
[ExecuteInEditMode]
public class AutoLayoutItem : MonoBehaviour
{
    [HideInInspector]
    public RectTransform Rect;

    AutoLayout auto;
    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        auto = transform.parent.GetComponent<AutoLayout>();
        if (auto)
        auto.AddItem(this);
    }
    public void IsChirent(AutoLayout layout)
    {
        if (layout.transform != transform.parent)
        {
            layout.RemoveItem(this);
        }
    }
    private void OnEnable()
    {
    }
    private void OnDestroy()
    {
        if (auto)
        {
            auto.RemoveItem(this);
        }
    }
}
