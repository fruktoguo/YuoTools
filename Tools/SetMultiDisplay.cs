using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMultiDisplay : MonoBehaviour
{
    RenderTexture renderTexture;
    public RawImage display1;
    public RawImage display2;
    public Camera mainCamera;
    void Awake()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 1);
        mainCamera.targetTexture = renderTexture;
        display1.texture = renderTexture;
        display2.texture = renderTexture;
    }
}
