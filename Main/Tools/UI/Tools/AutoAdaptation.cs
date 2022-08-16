using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using YuoTools;

public class AutoAdaptation : MonoBehaviour
{
    private RectTransform rect;
    public Vector2 DefSize = new Vector2(2160, 1080);
    public bool AutoY = false;

    // Start is called before the first frame update
    private void Start()
    {
        rect = transform as RectTransform;
        if (AutoY)
            rect.sizeDelta = rect.sizeDelta.RSetY(rect.sizeDelta.y * Screen.height / DefSize.y / Screen.width * DefSize.x);
        else
            rect.sizeDelta = rect.sizeDelta.RSetX(rect.sizeDelta.x * Screen.width / DefSize.x / Screen.height * DefSize.y);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}