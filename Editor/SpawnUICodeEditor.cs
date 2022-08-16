using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.Editor;
using YuoTools.Extend.UI;
using YuoTools.YuoEditor;

public class SpawnUICodeEditor
{
    // [MenuItem("GameObject/YuoUI/生成窗口UI代码", false, -2)]
    // public static void CreateUICode()
    // {
    //     foreach (var go in Selection.gameObjects)
    //     {
    //         SpawnUICode.SpawnCode(go);
    //     }
    // }

    [MenuItem("GameObject/YuoUI/创建UI", false, -2)]
    public static void CreateUI()
    {
        GameObject go = Resources.Load<GameObject>("YuoUI/UI_Window");
        go = GameObject.Instantiate(go, Selection.activeGameObject.transform);
        go.name = "New_UI_Window";
    }

    [MenuItem("GameObject/YuoUI命名/将UI的名字改成图片的名字", false, -2)]
    public static void ChangeNameForSprite()
    {
        foreach (var image in EditorTools.GetAllSelectComponent<Image>(true))
        {
            image.name = image.sprite?.name;
        }
    }

    [MenuItem("GameObject/YuoUI命名/将UI的名字改成文本", false, -2)]
    public static void ChangeNameForText()
    {
        foreach (var text in EditorTools.GetAllSelectComponent<Text>(true))
        {
            text.name = text.text;
        }
        foreach (var text in EditorTools.GetAllSelectComponent<TextMeshProUGUI>(true))
        {
            text.name = text.text;
        }
    }

    private static float _lastTime = int.MinValue;

    [MenuItem("GameObject/YuoUI命名/切换是否被框架检索_C", false, -2)]
    public static void ChangeNameForFrame()
    {
        if (_lastTime.ApEqual(Time.realtimeSinceStartup))
        {
            return;
        }

        _lastTime = Time.realtimeSinceStartup;
        foreach (var go in Selection.objects)
        {
            if (go.name.StartsWith("C_"))
                go.name = go.name.Replace("C_", "");
            else
            {
                go.name = "C_" + go.name;
            }
        }
    }

    [MenuItem("GameObject/YuoUI命名/切换UI子面板_D", false, -2)]
    public static void ChangeNameForChild()
    {
        if (_lastTime.ApEqual(Time.realtimeSinceStartup))
        {
            return;
        }

        _lastTime = Time.realtimeSinceStartup;
        foreach (var go in Selection.gameObjects)
        {
            if (go.name.StartsWith("D_"))
                go.name = go.name.Replace("D_", "");
            else
            {
                go.name = "D_" + go.name;
            }
        }
    }

    [MenuItem("GameObject/YuoUI命名/切换公共UI_G", false, -2)]
    public static void ChangeNameForG()
    {
        if (_lastTime.ApEqual(Time.realtimeSinceStartup))
        {
            return;
        }

        _lastTime = Time.realtimeSinceStartup;
        foreach (var go in Selection.gameObjects)
        {
            if (go.name.StartsWith("G_"))
                go.name = go.name.Replace("G_", "");
            else
            {
                go.name = "G_" + go.name;
            }
        }
    }
}