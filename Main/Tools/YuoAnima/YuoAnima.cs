using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace YuoTools
{
    [RequireComponent(typeof(Animator))]
    public class YuoAnima : SerializedMonoBehaviour
    {
        public Animator Animator { get; private set; }

        [Required("无对应动画文件,点击创建")] [HorizontalGroup("动画信息")]
        public YuoAnimaScriptable animaScriptable;

#if UNITY_EDITOR

        [HideIf("animaScriptable")]
        [HorizontalGroup("动画信息", width: 50)]
        [Button("创建", ButtonHeight = 60)]
        private void CreateAnimaScriptable()
        {
            var yas = ScriptableObject.CreateInstance<YuoAnimaScriptable>();
            var eac = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
            string path;
            if (eac != null)
            {
                yas.Init(eac);
                if (GetComponent<Animator>().GetBehaviour<YuoStateMachineBehaviour>() == null)
                {
                    eac.layers[0].stateMachine.AddStateMachineBehaviour<YuoStateMachineBehaviour>();
                }

                path = AssetDatabase.GetAssetPath(eac.GetInstanceID()).Replace($"{eac.name}.controller", "")
                       + $"{eac.name}_YuoAnimaScriptable" + ".asset";
            }
            else
            {
                var eavc = GetComponent<Animator>().runtimeAnimatorController as AnimatorOverrideController;
                if (eavc == null) return;
                yas.Init(eavc);
                if (GetComponent<Animator>().GetBehaviour<YuoStateMachineBehaviour>() == null)
                {
                    (eavc.runtimeAnimatorController as AnimatorController)?.layers[0].stateMachine
                        .AddStateMachineBehaviour<YuoStateMachineBehaviour>();
                }

                path = AssetDatabase.GetAssetPath(eavc.GetInstanceID()).Replace($"{eavc.name}.overrideController", "")
                       + $"{eavc.name}_YuoAnimaScriptable" + ".asset";
            }

            animaScriptable = yas;
            AssetDatabase.CreateAsset(yas, path);
            AssetDatabase.SaveAssets(); //存储资源
            AssetDatabase.Refresh(); //刷新
        }

        [ShowIf("animaScriptable")]
        [HorizontalGroup("动画信息", width: 50)]
        [Button("刷新")]
        private void RefreshAnimaScriptable()
        {
            animaScriptable.Refresh();
        }

#endif

        [ReadOnly] private Dictionary<string, YuoAnimaItem> animas = new Dictionary<string, YuoAnimaItem>();

        [ReadOnly] private Dictionary<int, string> hashToName = new Dictionary<int, string>();

        [ReadOnly] public YuoAnimaItem NowAnima;

        public YuoStateMachineBehaviour YuoStateMachineBehaviour { get; private set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            var data = Instantiate(animaScriptable);
            animas = data.Animas;
            hashToName = data.HashToName;
            YuoStateMachineBehaviour = Animator.GetBehaviour<YuoStateMachineBehaviour>();
            if (!YuoStateMachineBehaviour)
            {
                Debug.LogError($"请在 [AnimatorController] 的 [layer] 中添加行为 [YuoStateMachineBehaviour] ");
            }

            YuoStateMachineBehaviour.Init(OnStateEnter, OnStateExit, OnStateMove, OnStateUpdate);
        }

        public bool isUpdateMod;

        private void FixedUpdate()
        {
            if (!isUpdateMod)
                MUpdate(Time.fixedTime);
        }

        private void Update()
        {
            if (isUpdateMod)
            {
                MUpdate(Time.deltaTime);
            }
        }

        void MUpdate(float deltaTime)
        {
            if (NowAnima != null)
            {
                foreach (var item in NowAnima.Transitions)
                {
                    if (item.condition != null)
                    {
                        if (item.condition(item))
                        {
                            item.OnOver?.Invoke();
                            PlayAnima(item.to.AnimaName);
                        }
                    }
                }
            }

            if (_lagTime > 0)
            {
                Animator.speed = animaSpeed * _lagPower;
                _lagTime -= deltaTime;
                if (_lagTime <= 0)
                {
                    SetSpeed(animaSpeed);
                }
            }
        }

        private float animaSpeed = 1;

        public void SetSpeed(float speed)
        {
            animaSpeed = speed;
            Animator.speed = speed;
        }

        /// <summary>
        /// 在一段时间内动画变速
        /// </summary>
        /// <param name="time"></param>
        /// <param name="power"></param>
        public void Lag(float time = 0.1f, float power = 0.1f)
        {
            _lagTime = time;
            _lagPower = power;
        }

        private float _lagTime = -1;
        private float _lagPower = 0.1f;

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animaName"></param>
        private void PlayAnima(string animaName)
        {
            Animator.Play(animaName);
        }

        /// <summary>
        /// 根据Hash值获取对应动画名称
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string GetStateName(int hash)
        {
            if (hashToName.ContainsKey(hash))
            {
                return hashToName[hash];
            }

            return null;
        }

        public string GetStateName(AnimatorStateInfo state)
        {
            return GetStateName(state.shortNameHash);
        }

        YuoAnimaItem GetAnimaItem(AnimatorStateInfo state)
        {
            return GetAnimaItem(GetStateName(state));
        }

        YuoAnimaItem GetAnimaItem(string animaName)
        {
            if (animas.ContainsKey(animaName))
            {
                return animas[animaName];
            }

            return null;
        }

        private void OnStateEnter(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateEnter  {HashToName[stateInfo.shortNameHash]}");

            var clipName = GetStateName(stateInfo);
            if (animas.TryGetValue(clipName, out var nowAnima))
            {
                if (animas.ContainsKey(clipName)) NowAnima = nowAnima;
                foreach (var item in NowAnima.Events)
                {
                    item.num = 0;
                    if (item.eventType == AnimaEventType.Enter)
                    {
                        item.num++;
                        item.action?.Invoke();
                    }
                }
            }
            else
            {
                Debug.LogError($"{clipName}  没有找到");
            }
        }

        private void OnStateExit(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateExit  {HashToName[stateInfo.shortNameHash]}");

            if (NowAnima == null) return;
            var clipName = GetStateName(stateInfo);
            if (clipName == null || clipName != NowAnima.AnimaName) return;


            var animaItem = GetAnimaItem(clipName);

            UpdateAction(anima, stateInfo, layerIndex);

            ExitAction(animaItem, anima, stateInfo, layerIndex);
        }

        void ExitAction(YuoAnimaItem animaItem, Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var item in animaItem.Events)
            {
                if (item.eventType == AnimaEventType.Exit)
                {
                    item.num++;
                    item.action?.Invoke();
                }
            }
        }

        private void OnStateMove(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateMove  {HashToName[stateInfo.shortNameHash]}");
        }

        private void OnStateUpdate(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateUpdate  {HashToName[stateInfo.shortNameHash]}");

            if (NowAnima == null) return;

            if (GetStateName(stateInfo) != NowAnima.AnimaName) return;

            UpdateAction(anima, stateInfo, layerIndex);
        }

        void UpdateAction(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var item in NowAnima.Events)
            {
                switch (item.eventType)
                {
                    case AnimaEventType.Once:
                        if (item.num == 0 && stateInfo.normalizedTime >= item.time)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Loop:
                        if ((stateInfo.normalizedTime > item.num && stateInfo.normalizedTime % 1 >= item.time) ||
                            //防止动画速度过快的情况导致的事件不触发
                            stateInfo.normalizedTime > item.num + 1)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Update:
                        item.action?.Invoke();
                        break;
                }
            }
        }

        public void AddTransition(string[] from, string to, BoolAction<YuoTransition> condition,
            UnityAction onOver = null)
        {
            foreach (var item in from)
            {
                AddTransition(item, to, condition, onOver);
            }
        }

        public void AddTransition(string from, string to, BoolAction<YuoTransition> condition,
            UnityAction onOver = null)
        {
            if (animas.ContainsKey(from) && animas.ContainsKey(to))
            {
                YuoTransition transition = new YuoTransition(Animator, animas[from], animas[to]);
                animas[from].Transitions.Add(transition);
                transition.OnOver = onOver;
                transition.condition = condition;
            }
            else
            {
                Debug.LogError("包含了错误的state名称");
            }
        }

        #region 添加事件

        public YuoAnimaEvent AddEvent(string clip, UnityAction action)
        {
            if (animas.ContainsKey(clip))
            {
                YuoAnimaEvent animaEvent = new YuoAnimaEvent()
                {
                    clip = animas[clip].Clip,
                    action = action,
                };
                animas[clip].Events.Add(animaEvent);
                return animaEvent;
            }

            return null;
        }

        /// <summary>
        /// 添加事件在对应帧数
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="frame"></param>
        /// <param name="action"></param>
        /// <param name="eventType"></param>
        public YuoAnimaEvent AddEventOnFrame(string clip, int frame, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop)
        {
            if (animas.ContainsKey(clip))
            {
                frame.Clamp((int)(animas[clip].Clip.frameRate * animas[clip].Clip.length));
                return AddEvent(clip, frame / animas[clip].Clip.frameRate, action, eventType);
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
                return null;
            }
        }

        /// <summary>
        /// 添加事件在对应时间
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="eventType"></param>
        public YuoAnimaEvent AddEvent(string clip, float time, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop)
        {
            if (animas.ContainsKey(clip))
            {
                YuoAnimaEvent animaEvent = new YuoAnimaEvent()
                {
                    clip = animas[clip].Clip,
                    time = (time / animas[clip].Clip.length).RClamp(1),
                    action = action,
                    eventType = eventType
                };
                animas[clip].Events.Add(animaEvent);
                return animaEvent;
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
                return null;
            }
        }

        /// <summary>
        /// 添加事件在对应时间的百分比
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public YuoAnimaEvent AddEventRatio(string clip, float time, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop)
        {
            if (animas.ContainsKey(clip))
            {
                YuoAnimaEvent animaEvent = new YuoAnimaEvent()
                {
                    clip = animas[clip].Clip,
                    time = time,
                    action = action,
                    eventType = eventType
                };
                animas[clip].Events.Add(animaEvent);
                return animaEvent;
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
                return null;
            }
        }

        /// <summary>
        /// 添加事件,会在该动画的每一帧执行
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public YuoAnimaEvent AddEventOnUpdate(string clip, UnityAction action)
        {
            var e = AddEvent(clip, action);
            e.eventType = AnimaEventType.Update;
            return e;
        }

        /// <summary>
        ///  添加事件,会在该动画退出时执行
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public YuoAnimaEvent AddEventOnExit(string clip, UnityAction action)
        {
            var e = AddEvent(clip, action);
            e.eventType = AnimaEventType.Exit;
            return e;
        }

        /// <summary>
        ///  添加事件,会在该动画进入时执行
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public YuoAnimaEvent AddEventOnEnter(string clip, UnityAction action)
        {
            var e = AddEvent(clip, action);
            e.eventType = AnimaEventType.Enter;
            return e;
        }

        /// <summary>
        /// 走Unity自己的的动画事件系统
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="functionName"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public AnimationEvent OnClipAddEventFrame(string clip, string functionName, int frame)
        {
            if (animas.ContainsKey(clip))
            {
                frame.Clamp((int)(animas[clip].Clip.frameRate * animas[clip].Clip.length));
                return OnClipAddEventTime(clip, functionName, frame / animas[clip].Clip.frameRate);
            }

            return null;
        }

        /// <summary>
        /// 走Unity自己的的动画事件系统
        /// </summary>
        public AnimationEvent OnClipAddEventTime(string clip, string functionName, float time)
        {
            if (animas.ContainsKey(clip))
            {
                time.Clamp(animas[clip].Clip.length);
                var ae = new AnimationEvent()
                {
                    time = time,
                    functionName = functionName,
                };
                animas[clip].Clip.AddEvent(ae);
                return ae;
            }
            else
            {
                Debug.LogError($"不存在[{clip}]动画");
                return null;
            }
        }

        #endregion 添加事件

        [Serializable]
        public class YuoTransition
        {
            public Animator anima;
            public YuoAnimaItem from;
            public YuoAnimaItem to;
            public BoolAction<YuoTransition> condition;
            public UnityAction OnOver;

            public YuoTransition(Animator animator, YuoAnimaItem from, YuoAnimaItem to)
            {
                anima = animator;
                this.from = from;
                this.to = to;
            }
        }

        //[System.Serializable]
        public class YuoAnimaItem
        {
            public string AnimaName = "Null";

            public AnimationClip Clip;

            public UnityAction OnEnter;

            [ReadOnly] public readonly List<YuoAnimaEvent> Events = new List<YuoAnimaEvent>();

            public UnityAction OnExit;

            public readonly List<YuoTransition> Transitions = new List<YuoTransition>();
            public float Speed = 1;
        }

        [Serializable]
        public class YuoAnimaEvent
        {
            public AnimaEventType eventType = AnimaEventType.Loop;
            public float time;
            public AnimationClip clip;
            public UnityAction action;
            public int num;
        }

        public enum AnimaEventType
        {
            Loop = 0,
            Once = 1,
            Update = 2,
            Exit = 3,
            Enter = 4,
        }
    }
}