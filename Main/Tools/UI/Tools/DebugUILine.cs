#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugUILine : MonoBehaviour
{
    private static Vector3[] fourCorners = new Vector3[4];
    public bool DestroyOnGame;

    private void Awake()
    {
        if (DestroyOnGame) Destroy(this);
    }

    private void OnDrawGizmos()
    {
        foreach (MaskableGraphic g in FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.red;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
            }
        }
    }
}

#endif