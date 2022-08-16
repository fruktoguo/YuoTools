using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace YuoTools
{
    public abstract class YuoRoteMono : MonoBehaviour
    {
        protected float rote = 1;

        protected virtual void Awake()
        {
            if (rote < Time.deltaTime)
                rote = Time.deltaTime;
        }

        protected float timer = 0;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > rote)
            {
                Fixed(timer);
                timer = 0;
            }
        }

        public void SetRote(float rote)
        {
            if (rote < 0.0001f) this.rote = float.MaxValue;
            else this.rote = 1 / rote;
        }

        protected abstract void Fixed(float space);
    }
}