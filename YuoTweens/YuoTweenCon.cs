using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace YuoTools
{
    public class YuoTweenCon : SingletonMono<YuoTweenCon>
    {
        public void YuoStartCoroutine(IEnumerator enumerator) => StartCoroutine(enumerator);

        public void PlayTextUpAndFade(Text text, float upDis, float overTime) => YuoStartCoroutine(TextUpAndFade(text, upDis, overTime));

        public void PlayMoveTo(Transform tran, Vector3 end, float needTime, UnityAction EndAction = null, float delayTime = 0) => StartCoroutine(MoveTo(tran, end, needTime, EndAction, delayTime));
        public void RectMove(RectTransform rect,Vector2 dir,float needTime,float Distance,UnityAction EndAction = null) => StartCoroutine(IRectMove(rect,dir,needTime,Distance,EndAction));
        #region
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="UpdateAction"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        IEnumerator MoveTo(Transform tran, Vector3 end, float needTime, UnityAction EndAction = null, float delayTime = 0)
        {
            float moveSpeed =Vector3.Distance(end,tran.position) / needTime;
            yield return YuoWait.GetWait(delayTime);
            while (true)
            {
                yield return null;
                if (tran == null)
                {
                    yield break;
                }
                if (Vector3.Distance(tran.position,end)<= moveSpeed*Time.deltaTime)
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
        /// 使一个Text逐渐上移且透明
        /// </summary>
        /// <param name="text"></param>
        /// <param name="overTime"></param>
        /// <returns></returns>
        IEnumerator TextUpAndFade(Text text, float upDis, float overTime)
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

        public IEnumerator IRectMove(RectTransform rect,Vector2 dir,float needTime,float Distance ,UnityAction EndAction = null)
        {
            float timer = needTime;
            while (true)
            {
                yield return null;
                timer -= Time.deltaTime;
                rect.anchoredPosition += dir.normalized / needTime* Distance * Time.deltaTime;
                if (timer <= 0)
                {
                    EndAction?.Invoke();
                    yield break;
                }
            }
        }
        #endregion

    }
    public static class TweenEx
    {
        public static Transform MoveTo(this Transform tran, Vector3 end, float needTime, UnityAction EndAction = null, float delayTime = 0)
        {
            YuoTweenCon.Instance.PlayMoveTo(tran, end, needTime, EndAction, delayTime);
            return tran;
        }

    }
}