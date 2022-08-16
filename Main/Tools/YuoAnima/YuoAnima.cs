using System;
using UnityEngine;
using System.Collections;
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
        public Animator animator { get; private set; }

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

        [ReadOnly] private Dictionary<string, YuoAnimaItem> Animas = new Dictionary<string, YuoAnimaItem>();

        [ReadOnly] private Dictionary<int, string> HashToName = new Dictionary<int, string>();

        [ReadOnly] public YuoAnimaItem NowAnima;

        public YuoStateMachineBehaviour YuoStateMachineBehaviour { get; private set; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            var data = Instantiate(animaScriptable);
            Animas = data.Animas;
            HashToName = data.HashToName;
            YuoStateMachineBehaviour = animator.GetBehaviour<YuoStateMachineBehaviour>();
            if (!YuoStateMachineBehaviour)
            {
                Debug.LogError($"请在 [AnimatorController] 的 [layer] 中添加行为 [YuoStateMachineBehaviour] ");
            }

            YuoStateMachineBehaviour.Init(OnStateEnter, OnStateExit, OnStateMove, OnStateUpdate);
        }

        public static bool IsUpdateMod = false;

        private void FixedUpdate()
        {
            if (!IsUpdateMod)
                mUpdate(Time.fixedTime);
        }

        private void Update()
        {
            if (IsUpdateMod)
            {
                mUpdate(Time.deltaTime);
            }
        }

        void mUpdate(float DeltaTime)
        {
            if (NowAnima != null)
            {
                foreach (var item in NowAnima.transitions)
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
                animator.speed = animaSpeed * _lagPower;
                _lagTime -= DeltaTime;
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
            animator.speed = speed;
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
        /// <param name="name"></param>
        private void PlayAnima(string name)
        {
            animator.Play(name);
        }

        /// <summary>
        /// 根据Hash值获取对应动画名称
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string GetStateName(int hash)
        {
            if (HashToName.ContainsKey(hash))
            {
                return HashToName[hash];
            }

            return "null";
        }

        private void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Animas.ContainsKey(HashToName[stateInfo.shortNameHash]))
            {
                NowAnima = Animas[HashToName[stateInfo.shortNameHash]];
                foreach (var item in NowAnima.Events)
                {
                    item.num = 0;
                    if (item.eventType == AnimaEventType.enter)
                    {
                        item.num++;
                        item.action?.Invoke();
                    }
                }
            }
            else
            {
                Debug.LogError($"{HashToName[stateInfo.shortNameHash]}  没有找到");
            }
            //print($"OnStateEnter  {HashToName[stateInfo.shortNameHash]}");
        }

        private void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (NowAnima == null) return;
            foreach (var item in NowAnima.Events)
            {
                if (item.eventType == AnimaEventType.exit)
                {
                    item.num++;
                    item.action?.Invoke();
                }
            }
            //print($"OnStateExit  {HashToName[stateInfo.shortNameHash]}");
        }

        private void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateMove  {HashToName[stateInfo.shortNameHash]}");
        }

        private void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (NowAnima == null) return;
            foreach (var item in NowAnima.Events)
            {
                switch (item.eventType)
                {
                    case AnimaEventType.once:
                        if (item.num == 0 && stateInfo.normalizedTime > item.time)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.loop:
                        if (stateInfo.normalizedTime > item.num && stateInfo.normalizedTime % 1 > item.time)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.update:
                        item.action?.Invoke();
                        break;

                    default:
                        break;
                }
            }
            //print($"OnStateUpdate  {HashToName[stateInfo.shortNameHash]}");
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
            if (Animas.ContainsKey(from) && Animas.ContainsKey(to))
            {
                YuoTransition transition = new YuoTransition(animator, Animas[from], Animas[to]);
                Animas[from].transitions.Add(transition);
                transition.OnOver = onOver;
                transition.condition = condition;
            }
            else
            {
                Debug.LogError("包含了错误的state名称");
            }
        }

        #region 添加事件

        /// <summary>
        /// 添加事件在对应帧数
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="frame"></param>
        /// <param name="action"></param>
        public void AddEventOnFrame(string clip, int frame, UnityAction action,
            AnimaEventType eventType = AnimaEventType.loop)
        {
            if (Animas.ContainsKey(clip))
            {
                frame.Clamp((int) (Animas[clip].Clip.frameRate * Animas[clip].Clip.length));
                AddEvent(clip, frame / Animas[clip].Clip.frameRate, action, eventType);
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
            }
        }

        /// <summary>
        /// 添加事件在对应时间
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public void AddEvent(string clip, float time, UnityAction action,
            AnimaEventType eventType = AnimaEventType.loop)
        {
            if (Animas.ContainsKey(clip))
            {
                YuoAnimaEvent animaEvent = new YuoAnimaEvent()
                {
                    clip = Animas[clip].Clip,
                    time = (time / Animas[clip].Clip.length).RClamp(1),
                    action = action,
                    eventType = eventType
                };
                Animas[clip].Events.Add(animaEvent);
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
            }
        }

        public void AddEventRatio(string clip, float time, UnityAction action,
            AnimaEventType eventType = AnimaEventType.loop)
        {
            if (Animas.ContainsKey(clip))
            {
                YuoAnimaEvent animaEvent = new YuoAnimaEvent()
                {
                    clip = Animas[clip].Clip,
                    time = time,
                    action = action,
                    eventType = eventType
                };
                Animas[clip].Events.Add(animaEvent);
            }
            else
            {
                Debug.LogError($"不存在【{clip}】动画");
            }
        }

        public AnimationEvent OnClipAddEventFrame(string clip, string functionName, int frame)
        {
            if (Animas.ContainsKey(clip))
            {
                frame.Clamp((int) (Animas[clip].Clip.frameRate * Animas[clip].Clip.length));
                return OnClipAddEventTime(clip, functionName, frame / Animas[clip].Clip.frameRate);
            }

            return null;
        }

        public AnimationEvent OnClipAddEventTime(string clip, string functionName, float time)
        {
            if (Animas.ContainsKey(clip))
            {
                time.Clamp(Animas[clip].Clip.length);
                var ae = new AnimationEvent()
                {
                    time = time,
                    functionName = functionName,
                };
                Animas[clip].Clip.AddEvent(ae);
                return ae;
            }
            else
            {
                Debug.LogError($"不存在[{clip}]动画");
                return null;
            }
        }

        #endregion 添加事件

        [System.Serializable]
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

            [HideInInspector] public UnityAction OnEnter;

            [ReadOnly] public List<YuoAnimaEvent> Events = new List<YuoAnimaEvent>();

            [HideInInspector] public UnityAction OnExit;

            public List<YuoTransition> transitions = new List<YuoTransition>();
            public float Speed = 1;
        }

        [System.Serializable]
        public class YuoAnimaEvent
        {
            public AnimaEventType eventType = AnimaEventType.loop;
            public float time;
            public AnimationClip clip;
            public UnityAction action;
            public int num;
        }

        public enum AnimaEventType
        {
            loop = 0,
            once = 1,
            update = 2,
            exit = 3,
            enter = 4,
        }
    }
}