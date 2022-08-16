using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using YuoTools.Extend.UI;

[RequireComponent(typeof(Text))]
public class TranslateText : MonoBehaviour
{
    [HideInInspector]
    public Text text;

    private string _key;

    private void Awake()
    {
        text = GetComponent<Text>();
        _key = text.text;
    }
    
    public  void OnLanguageChanged()
    {
        text.text = _key.Language();
    }
}