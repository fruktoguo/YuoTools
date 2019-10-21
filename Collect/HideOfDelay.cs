using UnityEngine;
namespace YuoTools
{
    public class HideOfDelay : MonoBehaviour
    {
        public float Delay;
        void Start()
        {
            this.YuoDelay(() => gameObject.SetActive(false), Delay.Clamp());
        }
    }
}