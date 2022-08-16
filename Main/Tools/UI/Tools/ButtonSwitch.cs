using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using YuoTools;

public class ButtonSwitch : MonoBehaviour
{
    public Sprite SwirchSprite;
    private Sprite OldSprite;
    private Button btn;
    private bool Sate;
    public Action<bool> OnClick;
    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SwitchSate);
        OldSprite = btn.image.sprite;
    }

    private void SwitchSate()
    {
        Sate.Reverse();
        btn.image.sprite = Sate ? SwirchSprite : OldSprite;
        OnClick?.Invoke(Sate);
    }
}