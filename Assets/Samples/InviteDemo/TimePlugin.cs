using System;
using Pico.Platform;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    public class TimePlugin : MonoBehaviour
    {
        public Text timeValue;
        public Slider timeSlider;
        public DateTime beginTime;
        public DateTime endTime;

        private void Start()
        {
            var now = DateTime.Now;
            beginTime = now.AddHours(-1);
            endTime = now.AddHours(1);
            timeSlider.value = TimeUtil.DateTimeToSeconds(endTime);
            timeSlider.minValue = TimeUtil.DateTimeToSeconds(beginTime);
            timeSlider.maxValue = TimeUtil.DateTimeToSeconds(endTime);
            timeSlider.onValueChanged.AddListener(v => { timeValue.text = TimeUtil.SecondsToDateTime((long) v).ToString(); });
        }

        public DateTime Get()
        {
            return TimeUtil.SecondsToDateTime((long) (timeSlider.value));
        }
    }
}