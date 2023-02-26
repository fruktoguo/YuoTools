using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace YuoTools
{
    public class ImageColorOfTime1 : MonoBehaviour
    {
        public Gradient gradient;

        [HideLabel]
        // [HorizontalGroup("animaTime",20)]
        public NumType animaTimeNumType;

        [HorizontalGroup("animaTime")] public float animaTime = 1;

        [ShowIf("animaTimeNumType", NumType.随机)] [HorizontalGroup("animaTime")]
        public float animaTimeMax = 1;

        private Image image;
        private float timer = 0;
        private SpriteRenderer spriteRenderer;
        private Color defcolor;
        public float StartDelay;
        public float Cycle = 1;

        public enum NumType
        {
            常量 = 0,
            随机 = 1,
        }

        private void Start()
        {
            if (Cycle < animaTime)
            {
                Cycle = animaTime;
            }

            image = GetComponent<Image>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            defcolor = spriteRenderer ? spriteRenderer.color : image.color;

            if (spriteRenderer)
            {
                OnLife.AddListener(x => spriteRenderer.color = defcolor * gradient.Evaluate(x));
            }
            else
            {
                OnLife.AddListener(x => image.color = defcolor * gradient.Evaluate(x));
            }

            OnLife?.Invoke(0);
        }

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
            else if (timer < Cycle - 0.1f)
            {
                OnLife?.Invoke(timer / animaTime);
            }
        }

        public UnityEvent<float> OnLife;
    }
}