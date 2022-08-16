using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using static YuoTools.YuoAnima;

namespace YuoTools
{
    [CreateAssetMenu(menuName = "YuoTools/Create YuoAnimaScriptable")]
    public class YuoAnimaScriptable : SerializedScriptableObject
    {
        [ReadOnly]
        public Dictionary<string, YuoAnimaItem> Animas = new Dictionary<string, YuoAnimaItem>();

        [ReadOnly]
        public Dictionary<int, string> HashToName = new Dictionary<int, string>();

        public void ReComputeHash()
        {
            List<string> names = new List<string>(); ;
            foreach (var item in HashToName.Values)
            {
                names.Add(item);
            }
            HashToName.Clear();
            foreach (var item in names)
            {
                HashToName.Add(Animator.StringToHash(item), item);
            }
        }

#if UNITY_EDITOR

        private enum AnimatorType
        {
            Controller,
            Override
        }

        [EnumToggleButtons]
        [SerializeField]
        private AnimatorType animatorType = AnimatorType.Controller;

        [ShowIf("animatorType", AnimatorType.Controller)]
        [SerializeField]
        private UnityEditor.Animations.AnimatorController animatorController;

        [ShowIf("animatorType", AnimatorType.Override)]
        [SerializeField]
        private AnimatorOverrideController animatorOverrideController;

        public void Init(UnityEditor.Animations.AnimatorController controller)
        {
            animatorType = AnimatorType.Controller;
            animatorController = controller;
            Refresh();
        }

        public void Init(AnimatorOverrideController controller)
        {
            animatorType = AnimatorType.Override;
            animatorOverrideController = controller;
            Refresh();
        }

        [Button("刷新")]
        public void Refresh()
        {
            //var eac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            HashToName = new Dictionary<int, string>();
            Animas = new Dictionary<string, YuoAnimaItem>();
            switch (animatorType)
            {
                case AnimatorType.Controller:
                    foreach (var item in animatorController.layers[0].stateMachine.states)
                    {
                        HashToName.Add(item.state.nameHash, item.state.name);
                        var anima = new YuoAnimaItem();
                        anima.AnimaName = item.state.name;
                        anima.Clip = item.state.motion as AnimationClip;
                        Animas.Add(item.state.name, anima);
                    }
                    break;

                case AnimatorType.Override:
                    var eac = animatorOverrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                    foreach (var item in eac.layers[0].stateMachine.states)
                    {
                        HashToName.Add(item.state.nameHash, item.state.name);
                        var anima = new YuoAnimaItem();
                        anima.AnimaName = item.state.name;
                        anima.Clip = animatorOverrideController[item.state.motion as AnimationClip];
                        Animas.Add(item.state.name, anima);
                    }
                    break;

                default:
                    break;
            }
        }

#endif
    }
}