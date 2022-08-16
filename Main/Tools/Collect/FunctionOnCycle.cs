using UnityEngine;
using UnityEngine.Events;
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