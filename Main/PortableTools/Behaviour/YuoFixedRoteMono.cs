using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YuoTools
{
    public abstract class YuoFixedRoteMono : MonoBehaviour
    {
        public int fps { get; private set; } = 30;
        private float delay = 0.0333f;
        private float timer = 0;
        private float delayTimer = 0;

        private void Awake()
        {
            SetFps(fps);
        }

        public void SetFps(float fps)
        {
            this.fps = (int)fps;
            delay = 1f / fps.RClamp(1, fps);
        }

        private void FixedUpdate()
        {
            if (Time.time - timer >= delay)
            {
                YuoUpdate(Time.time - delayTimer);
                delayTimer = Time.time;
                timer += delay;
            }
        }

        protected abstract void YuoUpdate(float delay);
    }
}