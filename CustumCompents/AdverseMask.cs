using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AdverseMask : Mask
{
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (isActiveAndEnabled)
            return true;
        return !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
    }
}
