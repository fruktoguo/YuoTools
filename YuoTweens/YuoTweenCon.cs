using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class YuoTweenCon : SingletonMono<YuoTweenCon>
{
    public void YuoStartCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }

    public void PlayTextUpAndFade(Text text, float upDis, float overTime)
    {
        YuoStartCoroutine(TextUpAndFade(text, upDis, overTime));
    }

    public void PlayMoveTo(Transform tran, Vector3 end, float Speed, UnityAction EndAction = null, float delayTime = 0)
    {
        StartCoroutine(MoveTo(tran,end,Speed,EndAction,delayTime));
    }

    #region
    /// <summary>
    /// 移动到目标位置
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="end"></param>
    /// <param name="Speed"></param>
    /// <param name="UpdateAction"></param>
    /// <param name="EndAction"></param>
    /// <returns></returns>
    IEnumerator MoveTo(Transform tran, Vector3 end, float Speed, UnityAction EndAction = null,float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
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
            text.color =  text.color.AddColorFade(-fadeSpeed * Time.deltaTime);
            tran.anchoredPosition3D += tran.up* upSpeed * Time.deltaTime;
            if (overTime < 0)
            {
                yield break;
            }
        }
    }

    #endregion

}
