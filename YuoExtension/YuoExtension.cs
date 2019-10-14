
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public static class YuoExtension
{
    #region Text
    public static void PlayTextUpAndFade(this Text text, float upDis, float overTime)
    {
        YuoTweenCon.Instance.PlayTextUpAndFade(text, upDis, overTime);
    }
    public static void UpdateText(this Text text, string str)
    {
        text.text = str;
    }
    #endregion

    #region Color
    public static Color UpdateColorFade(this Color color, float fade)
    {
        fade = Mathf.Clamp(fade, 0, 1);
        color = new Color(color.r, color.g, color.b, fade);
        return color;
    }
    public static Color AddColorFade(this Color color, float fade)
    {
        float temp = fade + color.a;
        temp = Mathf.Clamp(temp, 0, 1);
        color = new Color(color.r, color.g, color.b, temp);
        return color;
    }
    #endregion

    #region Transform

    public static void ResetTrans(this Transform tran)
    {
        tran.position = Vector3.zero;
        tran.eulerAngles = Vector3.zero;
        tran.localScale = Vector3.one;
    }
    public static void ResetTransLocal(this Transform tran)
    {
        tran.localPosition = Vector3.zero;
        tran.localEulerAngles = Vector3.zero;
        tran.localScale = Vector3.one;
    }

    #region Position
    public static void SetPosX(this Transform tran, float PosX)
    {
        tran.position = new Vector3(PosX, tran.position.y, tran.position.z);
    }
    public static void SetPosY(this Transform tran, float PosY)
    {
        tran.position = new Vector3(tran.position.x, PosY, tran.position.z);
    }
    public static void SetPosZ(this Transform tran, float PosZ)
    {
        tran.position = new Vector3(tran.position.x, tran.position.y, PosZ);
    }
    public static bool InRange(this Vector2Int pos, Vector2Int zero, int MaxWidth,int MinWidth, int MaxHeight,int MinHeight)
    {
        (pos, MaxHeight, MinHeight, MaxWidth, MinWidth).Log();
        if (pos.x >= zero.x + MinWidth && pos.x < zero.x + MaxWidth && pos.y >= zero.y + MinHeight && pos.y < zero.y + MaxHeight / 2)
            return true;
        return false;
    }
    public static bool iInRange(this Vector2Int pos, Vector2Int zero, int width, int height)
    {
        (pos, width, height).Log();
        if ((pos.x >= zero.x - width / 2 && pos.x < zero.x + width / 2) && (pos.y >= zero.y - height / 2 && pos.y < zero.y + height / 2))
            return true;
        return false;
    }
    #endregion

    #endregion

    #region GameObject

    public static void Show(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    public static void Hide(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Animator

    public static float GetClipLength(this Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
        {
            tAnimationClip = ac.animationClips[tCounter];
            if (null != tAnimationClip && tAnimationClip.name == clip)
                return tAnimationClip.length;
        }
        return 0F;
    }

    #endregion

    #region Mathf
    public static int Clamp(ref this int i, int min, int max)
    {
        i = Mathf.Clamp(i, min, max);
        return i;
    }
    public static int Clamp(ref this int i, int max)
    {
        i = Mathf.Clamp(i, 0, max);
        return i;
    }
    public static int Clamp(ref this int i)
    {
        i = Mathf.Clamp(i, 0, i);
        return i;
    }
    public static float Clamp(ref this float i, float min, float max)
    {
        i = Mathf.Clamp(i, min, max);
        return i;
    }
    public static float Clamp(ref this float i, float max)
    {
        i = Mathf.Clamp(i, 0, max);
        return i;
    }
    public static float Clamp(ref this float i)
    {
        i = Mathf.Clamp(i, 0, i);
        return i;
    }
    #endregion

}

