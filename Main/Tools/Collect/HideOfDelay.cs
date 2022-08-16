using UnityEngine;

namespace YuoTools
{
    public class HideOfDelay : MonoBehaviour
    {
        public float Delay;

        private async void Start()
        {
            Delay.Clamp();
            await YuoWait.WaitTimeAsync(Delay);
            gameObject.SetActive(false);
        }
    }
}