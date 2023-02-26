using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools;

public class ButtonSwitchGo : MonoBehaviour
{
    public List<GameObject> onTure = new();
    public List<GameObject> onFalse = new();
    private Button btn;
    [OnValueChanged("OnSwitchSate")][SerializeField] private bool sate;
    public UnityEvent<bool> onClick;

    public bool Sate
    {
        get => sate;
        set
        {
            if (value != sate)
            {
                sate = value;
                OnSwitchSate();
            }
        }
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SwitchSate);
    }

    void OnSwitchSate()
    {
        if (Sate)
        {
            onTure.ShowAll();
            onFalse.HideAll();
        }
        else
        {
            onTure.HideAll();
            onFalse.ShowAll();
        }

        onClick?.Invoke(Sate);
    }

    private void SwitchSate()
    {
        sate.Reverse();
        OnSwitchSate();
    }
}