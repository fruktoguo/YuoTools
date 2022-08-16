using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace YuoTools
{
    public class YuoStateMachineBehaviour : StateMachineBehaviour
    {
        private UnityAction<Animator, AnimatorStateInfo, int> onStateEnter;
        private UnityAction<Animator, AnimatorStateInfo, int> onStateExit;
        private UnityAction<Animator, AnimatorStateInfo, int> onStateMove;
        private UnityAction<Animator, AnimatorStateInfo, int> onStateUpdate;

        public void Init(
            UnityAction<Animator, AnimatorStateInfo, int> onStateEnter,
            UnityAction<Animator, AnimatorStateInfo, int> onStateExit,
            UnityAction<Animator, AnimatorStateInfo, int> onStateMove,
            UnityAction<Animator, AnimatorStateInfo, int> onStateUpdate)
        {
            this.onStateEnter = onStateEnter;
            this.onStateExit = onStateExit;
            this.onStateMove = onStateMove;
            this.onStateUpdate = onStateUpdate;
        }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //base.OnStateEnter(animator, stateInfo, layerIndex);
            onStateEnter?.Invoke(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //base.OnStateExit(animator, stateInfo, layerIndex);
            onStateExit?.Invoke(animator, stateInfo, layerIndex);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //base.OnStateMove(animator, stateInfo, layerIndex);
            onStateMove?.Invoke(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //base.OnStateUpdate(animator, stateInfo, layerIndex);
            onStateUpdate?.Invoke(animator, stateInfo, layerIndex);
        }
    }
}