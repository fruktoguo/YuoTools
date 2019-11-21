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

        public void PlayMoveTo(Transform tran, Vector3 end, float Speed, UnityAction EndAction = null, float delayTime = 0) => StartCoroutine(MoveTo(tran, end, Speed, EndAction, delayTime));
        public YuoTweenModForRect RectMove(RectTransform rect, Vector2 dir, float needTime, float Distance, YuoTweenModForRect mod)
        {
            StartCoroutine(IRectMove(rect, dir, needTime, Distance, mod));
            return mod;
        }
        #region IEnumerator
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="Speed"></param>
        /// <param name="UpdateAction"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        IEnumerator MoveTo(Transform tran, Vector3 end, float Speed, UnityAction EndAction = null, float delayTime = 0)
        {
            yield return YuoWait.GetWait(delayTime);
            while (true)
            {
                yield return null;
                tran.position = Vector3.MoveTowards(tran.position, end, Speed);
                if (Vector3.Distance(tran.position, end) < 0.001f)
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

        public IEnumerator IRectMove(RectTransform rect, Vector2 dir, float needTime, float Distance, YuoTweenModForRect mod)
        {
            float timer = needTime;
            while (true)
            {
                yield return null;
                timer -= Time.deltaTime;
                rect.anchoredPosition += dir.normalized / needTime * Distance * Time.deltaTime;
                if (timer <= 0)
                {
                    if (mod.Upend)
                    {
                        timer = needTime;
                        mod.Upend = false;
                        dir *= -1;
                    }
                    else
                    {
                        mod.EndAction?.Invoke();
                        yield break;
                    }
                }
            }
        }
        #endregion

        public class YuoTweenMod
        {
            public UnityAction EndAction;
        }
        public class YuoTweenModForRect : YuoTweenMod
        {
            public bool Upend;
            public YuoTweenModForRect SetUpend()
            {
                Upend = true;
                return this;
            }
        }
    }
}