using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace YuoTools
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class YuoAnimator : MonoBehaviour
    {
        Dictionary<string, AnimaActionItem> Actions = new Dictionary<string, AnimaActionItem>();
        [Header("所有的事件")]
        public List<AnimaActionItem> AllActions = new List<AnimaActionItem>();
        Animator animator;
        public virtual void Awake()
        {
            animator = GetComponent<Animator>();
            foreach (var item in AllActions)
            {
                Actions.Add(item.animaClipName,item);
            }
        }
        List<YuoDelayMod> allMod = new List<YuoDelayMod>();
        public void Play(string clipName)
        {
            if (Actions.ContainsKey(clipName))
            {
                //animator.Play(clipName);
                foreach (var item in allMod)
                {
                    YuoDelayCon.Instance.StopCor(item);
                }
                allMod.Clear();
                foreach (var item in Actions[clipName].actions)
                {
                    allMod.Add(this.YuoDelay(item.action.Invoke, item.time ).SetName($"[ {gameObject.name} ]的动画片段 [ {clipName} ]在第{item.time}秒时的事件"));
                }
            }
        }
        public void Play(string clipName,float speed)
        {
            if (Actions.ContainsKey(clipName))
            {
                //animator.Play(clipName);
                foreach (var item in allMod)
                {
                    YuoDelayCon.Instance.StopCor(item);
                }
                allMod.Clear();
                animator.SetFloat(clipName, speed);
                foreach (var item in Actions[clipName].actions)
                {
                    allMod.Add(this.YuoDelay(item.action.Invoke, item.time / speed ).SetName($"[ {gameObject.name} ]的动画片段 [ {clipName} ]在第{item.time}秒时的事件"));
                }
            }
        }
        AnimaActionItem itemTemp;
        public void Add(string clipName, float time, UnityAction action)
        {
            if (!Actions.ContainsKey(clipName))
            {
                itemTemp = new AnimaActionItem(clipName);
                AllActions.Add(itemTemp);
                Actions.Add(clipName, itemTemp);
            }
            else itemTemp = Actions[clipName];
            itemTemp.actions.Add(new AnimaAction(time, action));
        }
        [System.Serializable]
        public class AnimaActionItem
        {
            [Header("动画片段名字")]
            public string animaClipName;
            [Header("具体事件")]
            public List<AnimaAction> actions = new List<AnimaAction>();

            public AnimaActionItem(string animaClipName)
            {
                this.animaClipName = animaClipName;
            }
        }

        [System.Serializable]
        public class AnimaAction
        {
            [Header("执行时间")]
            public float time;
            [Header("具体事件")]
            public YuoAction action = new YuoAction();

            public AnimaAction(float time, UnityAction action)
            {
                this.time = time;
                this.action.AddListener(action);
            }
        }
    }
}
