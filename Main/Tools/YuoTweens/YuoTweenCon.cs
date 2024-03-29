﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YuoTools
{
    public class YuoTweenCon : SingletonMono<YuoTweenCon>
    {
        private Dictionary<string, Coroutine> coroutinePools = new Dictionary<string, Coroutine>();

        public static void Stop(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static void Stop(string name)
        {
            if (Instance.coroutinePools.ContainsKey(name))
            {
                Instance.StopCoroutine(Instance.coroutinePools[name]);
                Instance.coroutinePools.Remove(name);
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
        public IEnumerator MoveTo(Transform tran, Vector3 end, float needTime, UnityAction EndAction = null,
            float delayTime = 0)
        {
            float moveSpeed = Vector3.Distance(end, tran.position) / needTime;
            yield return YuoWait.WaitTime(delayTime);
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
        /// 移动到目标点(曲线)
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="con"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        public IEnumerator MoveToCurue(Transform tran, Vector3 end, float needTime, Vector3 con,
            UnityAction EndAction = null)
        {
            float moveSpeed = 1 / needTime;
            needTime = 0;
            Vector3 start = tran.position;
            while (true)
            {
                yield return null;
                if (tran == null)
                {
                    yield break;
                }

                if (needTime >= 1)
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

        public IEnumerator IRectMove(RectTransform rect, Vector2 dir, float needTime, float Distance,
            UnityAction EndAction = null)
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

        public IEnumerator IFloatTo(float start, float end, float needTime, FloatTo action, float addSpeed = 0,
            UnityAction EndAction = null, string name = "")
        {
            float speed = (end - start) / needTime;
            float speedTemp = speed;
            addSpeed *= speed;
            float timer = needTime;
            while (true)
            {
                yield return null;
                speed += addSpeed * Time.unscaledDeltaTime;
                //speed.Clamp(speedTemp * 0.1f, speed);
                timer -= Time.unscaledDeltaTime;
                action?.Invoke(end - timer * speed);
                if (timer <= 0)
                {
                    action?.Invoke(end);
                    EndAction?.Invoke();
                    Stop(name);
                    yield break;
                }
            }
        }

        public IEnumerator IIntTo(int start, int end, float needTime, IntTo action, float addSpeed = 0,
            UnityAction EndAction = null)
        {
            float speed = (end - start) / needTime;
            float speedTemp = speed;
            addSpeed *= speed;
            float timer = needTime;
            int value = start;
            while (true)
            {
                yield return null;
                speed += addSpeed * Time.unscaledDeltaTime;
                //speed.Clamp(speedTemp * 0.1f, speed);
                timer -= Time.unscaledDeltaTime;
                value = (int) (end - timer * speed);
                if (value != start)
                {
                    action?.Invoke(value);
                    start = value;
                }

                if (timer <= 0)
                {
                    action?.Invoke(end);
                    EndAction?.Invoke();
                    yield break;
                }
            }
        }

        public IEnumerator IV3To(Vector3 start, Vector3 end, float needTime, V3To action, UnityAction EndAction = null)
        {
            Vector3 speed = (end - start) / needTime;
            float timer = needTime;
            while (true)
            {
                yield return null;
                timer -= Time.deltaTime;
                action?.Invoke(end - timer * speed);
                if (timer <= 0)
                {
                    action?.Invoke(end);
                    EndAction?.Invoke();
                    yield break;
                }
            }
        }

        public delegate void FloatTo(float value);

        public delegate void IntTo(int value);

        public delegate void V3To(Vector3 value);

        public void StartTween(IEnumerator enumerator, string name)
        {
            if (coroutinePools.ContainsKey(name))
            {
                StopCoroutine(coroutinePools[name]);
                coroutinePools[name] = StartCoroutine(enumerator);
            }
            else
            {
                coroutinePools.Add(name, StartCoroutine(enumerator));
            }
        }
    }

    public static class TweenEx
    {
        /// <summary>
        /// 移动到目标点
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="EndAction"></param>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public static Coroutine MoveTo(this Transform tran, Vector3 end, float needTime, UnityAction EndAction = null,
            float delayTime = 0)
        {
            return YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.MoveTo(tran, end, needTime, EndAction,
                delayTime));
        }

        /// <summary>
        /// 移动到目标点(曲线)
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="end"></param>
        /// <param name="needTime"></param>
        /// <param name="con"></param>
        /// <param name="EndAction"></param>
        /// <returns></returns>
        public static Coroutine MoveToCurue(this Transform tran, Vector3 end, float needTime, Vector3 con,
            UnityAction EndAction = null)
        {
            return YuoTweenCon.Instance.StartCoroutine(
                YuoTweenCon.Instance.MoveToCurue(tran, end, needTime, con, EndAction));
        }

        public static Coroutine MoveToCurue2D(this Transform tran, Vector2 end, float needTime, float conx, float cony,
            UnityAction EndAction = null)
        {
            return YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.MoveToCurue(tran, end, needTime,
                GetConPostion(tran.position, end, cony, conx), EndAction));
        }

        public static Vector2 GetConPostion(Vector2 start, Vector2 end, float x, float y)
        {
            Temp.V2 = (end + start) / 2;
            Temp.V2 *= y;
            Temp.V2.Set(Temp.V2.x + start.y - end.y, Temp.V2.y + end.x - start.x);
            Temp.V2 = (Temp.V2 - ((end + start) / 2 * y)) * x + (start + (end - start) * y);
            return Temp.V2;
        }

        public static void To(this float f, string name, float value, float needTime, YuoTweenCon.FloatTo floatTo,
            float addSpeed = 0, UnityAction action = null)
        {
            YuoTweenCon.Instance.StartTween(
                YuoTweenCon.Instance.IFloatTo(f, value, needTime, floatTo, addSpeed, action, name), name);
        }

        public static Coroutine To(this float f, float value, float needTime, YuoTweenCon.FloatTo floatTo,
            float addSpeed = 0, UnityAction action = null)
        {
            return YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.IFloatTo(f, value, needTime, floatTo,
                addSpeed, action));
        }

        // public static Coroutine To(this int f, int value, float needTime, YuoTweenCon.IntTo floatTo, float addSpeed = 0,
        //     UnityAction action = null)
        // {
        //     return YuoTweenCon.Get.StartCoroutine(YuoTweenCon.Get.IIntTo(f, value, needTime, floatTo,
        //         addSpeed, action));
        // }

        public static void To(this ref Vector3 f, Vector3 value, float needTime, YuoTweenCon.V3To floatTo,
            UnityAction action = null)
        {
            YuoTweenCon.Instance.StartCoroutine(YuoTweenCon.Instance.IV3To(f, value, needTime, floatTo, action));
        }
    }
}