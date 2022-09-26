using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.SportCenter
{
    public class SliderText : MonoBehaviour
    {
        public bool unitIsDay;
        private Text text;
        static readonly DateTime BEGIN_TIME = DateTime.Now;
        private Slider slider;
        public DateTime value = BEGIN_TIME;

        void Start()
        {
            slider = GetComponentInChildren<Slider>();
            text = GetComponentInChildren<Text>();
            if (unitIsDay)
            {
                slider.minValue = 0;
                slider.maxValue = 90;
            }
            else
            {
                slider.minValue = 0;
                slider.maxValue = 24;
            }

            text.text = $"{value}";
            slider.onValueChanged.AddListener(v =>
            {
                if (unitIsDay)
                {
                    value = BEGIN_TIME.Subtract(TimeSpan.FromDays(v));
                }
                else
                {
                    value = BEGIN_TIME.Subtract(TimeSpan.FromHours(v));
                }

                text.text = $"{value}";
            });
        }
    }
}