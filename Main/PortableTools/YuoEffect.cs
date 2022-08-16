using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace YuoTools
{
    public class YuoEffect : MonoBehaviour
    {
        public float animaTime = 1;
        private float timer = 0;
        public float StartDelay;
        public float Cycle;

        private void OnEnable()
        {
            timer = -StartDelay;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer <= animaTime && timer >= 0)
            {
                OnLife?.Invoke(timer / animaTime);
            }
            else if (timer > Cycle)
            {
                timer = 0;
            }
        }

        public UnityEvent<float> OnLife;
    }
}