using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;

[RequireComponent(typeof(Image))]
public class TranslateImage : MonoBehaviour
{
    [EnumToggleButtons] public TranType tranType;

    [ShowIf("tranType", TranType.Path)] public string path;

    [ShowIf("tranType", TranType.Sprite)]
    public YuoDictionary<LanguageManager.LanType, Sprite> tranSprite = new();

    public enum TranType
    {
        /// <summary>
        /// 路径
        /// </summary>
        Path = 0,

        /// <summary>
        /// 精灵
        /// </summary>
        Sprite = 1
    }

    [HideInInspector] public Image image;

    public void Initialization()
    {
        image = GetComponent<Image>();
    }
}