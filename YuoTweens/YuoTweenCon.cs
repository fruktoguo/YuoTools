using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace YuoTools
{
    public class YuoTweenCon : SingletonMono<YuoTweenCon>
    {
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="UpdateAction"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        public IEnumerator MoveTo(Transform tran, Vector3 end, float needTime, UnityAction EndAction = null, float delayTime = 0)
        {
            float moveSpeed = Vector3.Distance(end, tran.position) / needTime;
            yield return YuoWait.GetWait(delayTime);
            while (true)
            {
                yield return null;
                if (tran == null)
                {
                    yield break;
                }
                if (Vector3.Distance(tran.position, end) <= moveSpeed * Time.deltaTime)
                {
                    tran.position = end;
                    EndAction?.Invoke();
                    yield break;
                }
                tran.position += (end - tran.position).normalized * moveSpeed * Time.deltaTime;
                if (Vector3.Distance(tran.position, end) < 0.01f)
                {
                    EndAction?.Invoke();
                    yield break;
                }
            }
        }        
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="UpdateAction"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        public IEnumerator MoveToCurue(Transform tran, Vector3 end, float needTime, Vector3 con ,UnityAction EndAction = null)
        {
            float moveSpeed =1 / needTime ;
            needTime = 0;
            Vector3 start = tran.position;
            Temp.V3 = start + end;
            Temp.V3 /= 2;
            Temp.V3.x *= 1+ con.x;
            Temp.V3.y *= 1+ con.y;
            Temp.V3.z *= 1+ con.z;
            con = Temp.V3;
            while (true)
            {
                yield return null;
                if (tran == null)
                {
                    yield break;
                }
                if (needTime>=1)
                {
                    tran.position = end;
                    EndAction?.Invoke();
                    yield break;
                }
                tran.position = YuoTool.CalculateCubicBezierPoint(needTime, start, con, end);
                needTime += moveSpeed * Time.deltaTime;
            }
        }
        /// <summary>
        /// 使一个Text逐渐上移且透明
        /// </summary>
        /// <param name="text"></param>
        /// <param name="overTime"></param>
        /// <returns></returns>
        public IEnumerator TextUpAndFade(Text text, float upDis, float overTime)
        {
            if (overTime <= 0)
            {
                Debug.LogError("不能输入小于等于0的数");
                yield break;
            }
            float fadeSpeed = 1 / overTime;
            float upSpeed = upDis / overTime;
            RectTransform tran = text.GetComponent<RectTransform>();
            while (true)
            {
                overTime -= Time.deltaTime;
                yield return null;
                text.color = text.color.AddColorFade(-fadeSpeed * Time.deltaTime);
                tran.anchoredPosition3D += tran.up * upSpeed * Time.deltaTime;
                if (overTime < 0)
                {
                    yield break;
                }
            }
        }
        public IEnumerator IRectMove(RectTransform rect, Vector2 dir, float needTime, float Distance, UnityAction EndAction = null)
        {
            float timer = needTime;
            while (true)
            {
                yield return null;
                timer -= Time.deltaTime;
                rect.anchoredPosition += dir.normalized / needTime * Distance * Time.deltaTime;
                if (timer <= 0)
                {
                    EndAction?.Invoke();
                    yield break;
                }
            }
        }
    }
    public static class TweenEx
    {
        public static Transform MoveTo(this Transform tran, Vector3 end, float needTime, UnityAction EndAction = null, float delayTime = 0)
        {
            YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.MoveTo(tran, end, needTime, EndAction, delayTime));
            return tran;
        }
        public static Transform MoveToCurue(this Transform tran, Vector3 end, float needTime, Vector3 con, UnityAction EndAction = null)
        {
            YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.MoveToCurue(tran, end, needTime, con, EndAction));
            return tran;
        }

    }
}