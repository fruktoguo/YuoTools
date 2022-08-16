using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;

namespace YuoTools
{
    public class OnBehaviour : MonoBehaviour
    {
#pragma warning disable CS0414

        [EnumToggleButtons, HideLabel]
        [SerializeField]
        private BehaviourType ShowType = BehaviourType.Awake;

#pragma warning restore CS0414

        [ShowIf("@(this.ShowType& BehaviourType.Awake)==BehaviourType.Awake")]
        public UnityEvent _Awake;

        [ShowIf("@(this.ShowType& BehaviourType.Start)==BehaviourType.Start")]
        public UnityEvent _Start;

        [ShowIf("@(this.ShowType& BehaviourType.OnEnable)==BehaviourType.OnEnable")]
        public UnityEvent _OnEnable;

        [ShowIf("@(this.ShowType& BehaviourType.OnDisable)==BehaviourType.OnDisable")]
        public UnityEvent _OnDisable;

        [ShowIf("@(this.ShowType& BehaviourType.OnDestroy)==BehaviourType.OnDestroy")]
        public UnityEvent _OnDestroy;

        private void OnEnable()
        {
            _OnEnable?.Invoke();
        }

        private void OnDisable()
        {
            _OnDisable?.Invoke();
        }

        private void OnDestroy()
        {
            _OnDestroy?.Invoke();
        }

        private void Start()
        {
            _Start?.Invoke();
        }

        private void Awake()
        {
            _Awake?.Invoke();
        }

        [System.Flags]
        private enum BehaviourType
        {
            Awake = 1 << 1,
            Start = 1 << 2,
            OnEnable = 1 << 3,
            OnDisable = 1 << 4,
            OnDestroy = 1 << 5,
            All = Awake | Start | OnEnable | OnDisable | OnDestroy
        }
    }
}

#endif