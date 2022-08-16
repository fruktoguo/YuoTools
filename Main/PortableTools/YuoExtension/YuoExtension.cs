using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public static class YuoExtension
    {
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

        public static Color Set(this ref Color color, float r, float g, float b, float a)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            color.a = a;
            return color;
        }

        public static Color SetAlpha(this ref Color color, float a)
        {
            color.a = a;
            return color;
        }

        public static Color SetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        public static Color SetColorAlpha(this SpriteRenderer sprite, float alpha)
        {
            sprite.color = Temp.color.SetAlpha(alpha);
            return sprite.color;
        }

        #endregion Color

        #region Transform

        public static void ResetTrans(this Transform tran)
        {
            tran.localPosition = Vector3.zero;
            tran.localEulerAngles = Vector3.zero;
            tran.localScale = Vector3.one;
        }

        public static void ResetTrans(this GameObject gameObject)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
        }

        public static Transform Show(this Transform tran)
        {
            tran.gameObject.SetActive(true);
            return tran;
        }

        public static Transform Hide(this Transform tran)
        {
            tran.gameObject.SetActive(false);
            return tran;
        }

        public static void SetPos(this RectTransform tran, float x, float y)
        {
            Temp.V2.Set(x, y);
            tran.anchoredPosition = Temp.V2;
        }

        public static void SetPos(this Transform tran, float x, float y, float z)
        {
            Temp.V3.Set(x, y, z);
            tran.position = Temp.V3;
        }

        public static Vector3 SetX(this ref Vector3 v3, float x)
        {
            Temp.V3.Set(x, v3.y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetY(this ref Vector3 v3, float y)
        {
            Temp.V3.Set(v3.x, y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetZ(this ref Vector3 v3, float z)
        {
            Temp.V3.Set(v3.x, v3.y, z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector2 SetX(this ref Vector2 V2, float x)
        {
            Temp.V2.Set(x, V2.y);
            V2 = Temp.V2;
            return V2;
        }

        public static Vector2 SetY(this ref Vector2 V2, float y)
        {
            Temp.V2.Set(V2.x, y);
            V2 = Temp.V2;
            return V2;
        }

        public static Vector2 RSetX(this Vector2 V2, float x)
        {
            Temp.V2.Set(x, V2.y);
            V2 = Temp.V2;
            return V2;
        }

        public static Vector2 RSetY(this Vector2 V2, float y)
        {
            Temp.V2.Set(V2.x, y);
            V2 = Temp.V2;
            return V2;
        }

        public static Vector2 AddX(this ref Vector2 v2, float x)
        {
            return v2.SetX(v2.x + x);
        }

        public static Vector2 AddY(this ref Vector2 v2, float y)
        {
            return v2.SetY(v2.y + y);
        }

        public static Vector2 RAddX(this Vector2 v2, float x)
        {
            return v2.SetX(v2.x + x);
        }

        public static Vector2 RAddY(this Vector2 v2, float y)
        {
            return v2.SetY(v2.y + y);
        }

        public static Vector3 AddX(this ref Vector3 v3, float x)
        {
            return v3.SetX(v3.x + x);
        }

        public static Vector3 AddY(this ref Vector3 v3, float y)
        {
            return v3.SetY(v3.y + y);
        }

        public static Vector3 AddZ(this ref Vector3 v3, float z)
        {
            return v3.SetZ(v3.z + z);
        }

        public static Vector3 RAddX(this Vector3 v3, float x)
        {
            return v3.SetX(v3.x + x);
        }

        public static Vector3 RAddY(this Vector3 v3, float y)
        {
            return v3.SetY(v3.y + y);
        }

        public static Vector3 RAddZ(this Vector3 v3, float z)
        {
            return v3.SetZ(v3.z + z);
        }

        public static Vector3 RSetX(this Vector3 v3, float x)
        {
            Temp.V3.Set(x, v3.y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 RSetY(this Vector3 v3, float y)
        {
            Temp.V3.Set(v3.x, y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 RSetZ(this Vector3 v3, float z)
        {
            Temp.V3.Set(v3.x, v3.y, z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetPos(this ref Vector3 v3, float x, float y, float z)
        {
            Temp.V3.Set(x, y, z);
            v3 = Temp.V3;
            return v3;
        }

        #endregion Transform

        #region RectTransform

        public static void Copy(this RectTransform target, RectTransform from)
        {
            target.localScale = from.localScale;
            target.anchorMin = from.anchorMin;
            target.anchorMax = from.anchorMax;
            target.pivot = from.pivot;
            target.sizeDelta = from.sizeDelta;
            target.anchoredPosition3D = from.anchoredPosition3D;
        }

        public static void FullScreen(this RectTransform target, bool resetScaleToOne)
        {
            if (resetScaleToOne) target.ResetLocalScaleToOne();
            target.AnchorMinToZero();
            target.AnchorMaxToOne();
            target.CenterPivot();
            target.SizeDeltaToZero();
            target.ResetAnchoredPosition3D();
            target.ResetLocalPosition();
        }

        public static void Center(this RectTransform target, bool resetScaleToOne)
        {
            if (resetScaleToOne) target.ResetLocalScaleToOne();
            target.AnchorMinToCenter();
            target.AnchorMaxToCenter();
            target.CenterPivot();
            target.SizeDeltaToZero();
        }

        public static void ResetAnchoredPosition3D(this RectTransform target)
        {
            target.anchoredPosition3D = Vector3.zero;
        }

        public static void ResetLocalPosition(this RectTransform target)
        {
            target.localPosition = Vector3.zero;
        }

        public static void ResetLocalScaleToOne(this RectTransform target)
        {
            target.localScale = Vector3.one;
        }

        public static void AnchorMinToZero(this RectTransform target)
        {
            target.anchorMin = Vector2.zero;
        }

        public static void AnchorMinToCenter(this RectTransform target)
        {
            target.anchorMin = Vector2.one * 0.5f;
        }

        public static void AnchorMaxToOne(this RectTransform target)
        {
            target.anchorMax = Vector2.one;
        }

        public static void AnchorMaxToCenter(this RectTransform target)
        {
            target.anchorMax = Vector2.one * 0.5f;
        }

        public static void CenterPivot(this RectTransform target)
        {
            target.pivot = Vector2.one * 0.5f;
        }

        public static void SizeDeltaToZero(this RectTransform target)
        {
            target.sizeDelta = Vector2.zero;
        }

        #endregion

        #region Position

        public static Vector3 SetPosX(this Transform tran, float PosX)
        {
            Temp.V3.Set(PosX, tran.position.y, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetPosY(this Transform tran, float PosY)
        {
            Temp.V3.Set(tran.position.x, PosY, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetPosZ(this Transform tran, float PosZ)
        {
            Temp.V3.Set(tran.position.x, tran.position.y, PosZ);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetLocalPosX(this Transform tran, float PosX)
        {
            Temp.V3.Set(PosX, tran.localPosition.y, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 SetLocalPosY(this Transform tran, float PosY)
        {
            Temp.V3.Set(tran.localPosition.x, PosY, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 SetLocalPosZ(this Transform tran, float PosZ)
        {
            Temp.V3.Set(tran.localPosition.x, tran.localPosition.y, PosZ);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static bool InRange(this Vector2Int pos, Vector2Int zero, int MaxWidth, int MinWidth, int MaxHeight,
            int MinHeight)
        {
            if (pos.x >= zero.x + MinWidth && pos.x < zero.x + MaxWidth && pos.y >= zero.y + MinHeight &&
                pos.y < zero.y + MaxHeight / 2)
                return true;
            return false;
        }

        public static bool iInRange(this Vector2Int pos, Vector2Int zero, int width, int height)
        {
            if ((pos.x >= zero.x - width / 2 && pos.x < zero.x + width / 2) &&
                (pos.y >= zero.y - height / 2 && pos.y < zero.y + height / 2))
                return true;
            return false;
        }

        #endregion Position

        #region GameObject

        public static GameObject Show(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            if (!gameObject.activeSelf) gameObject.SetActive(true);
            return gameObject;
        }

        public static GameObject Hide(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            if (gameObject.activeSelf) gameObject.SetActive(false);
            return gameObject;
        }

        public static GameObject ReShow(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            gameObject.SetActive(false);
            gameObject.SetActive(true);
            return gameObject;
        }

        #endregion GameObject

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

        #endregion Animator

        #region Enum

        public static System.Array GetAll<T>(this T _enum) where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T));
        }

        #endregion Enum

        #region Main

        public static void Adds<T>(this List<T> list, params T[] t)
        {
            list.AddRange(t);
        }

        #endregion
    }
}