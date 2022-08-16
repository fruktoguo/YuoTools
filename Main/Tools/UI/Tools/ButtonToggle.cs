using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonToggle : MonoBehaviour, IPointerClickHandler
{
    public ButtonToggleGroup toggleGroup;

    [HideInInspector]
    public RectTransform rect;

    public bool IsOn;

    public void OnPointerClick(PointerEventData eventData)
    {
        toggleGroup.SwitchToggle(this);
    }

    private void Awake()
    {
        rect = transform as RectTransform;
        toggleGroup.toggles.Add(this);
    }

    public UnityAction<bool> OnValueChange;
}