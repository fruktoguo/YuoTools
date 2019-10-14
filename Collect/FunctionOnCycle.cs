using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace YuoTools
{
    public class FunctionOnCycle : MonoBehaviour
    {
        public UnityEvent start;
        public UnityEvent update;
        public UnityEvent destroy;
        // Start is called before the first frame update
        void Start()
        {
            start?.Invoke();
        }
        // Update is called once per frame
        void Update()
        {
            update?.Invoke();
        }
        void OnDestroy()
        {
            destroy?.Invoke();
        }
    }
}