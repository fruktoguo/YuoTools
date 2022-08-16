using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using YuoTools;

[RequireComponent(typeof(YuoEffect))]
public class PosOfTime : MonoBehaviour
{
    public Vector2 StartPos;
    public Vector2 EndPos;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        GetComponent<YuoEffect>().OnLife.AddListener(OnLife);
    }

    private void OnLife(float value)
    {
        rectTransform.anchoredPosition = Vector2.Lerp(StartPos, EndPos, value);
    }
}