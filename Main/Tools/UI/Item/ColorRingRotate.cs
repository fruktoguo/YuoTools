using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YuoTools
{
    public class ColorRingRotate : MonoBehaviour
    {
        Image image;

        private float defFade;

        void Start()
        {
            image = GetComponent<Image>();
            defFade = image.color.a;
        }

        float hue = 0;

        public float speed = 1;

        // Update is called once per frame
        void Update()
        {
            image.color = Color.HSVToRGB(hue, 1, 1).RSetA(defFade);
            hue += Time.deltaTime * speed * 0.1f;
            if (hue > 1)
            {
                hue = 0;
            }
        }
    }
}