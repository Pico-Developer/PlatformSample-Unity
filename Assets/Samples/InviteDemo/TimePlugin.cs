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
            timeSlider.value = Util.DateTimeToSeconds(endTime);
            timeSlider.minValue = Util.DateTimeToSeconds(beginTime);
            timeSlider.maxValue = Util.DateTimeToSeconds(endTime);
            timeSlider.onValueChanged.AddListener(v => { timeValue.text = Util.SecondsToDateTime((long) v).ToString(); });
        }

        public DateTime Get()
        {
            return Util.SecondsToDateTime((long) (timeSlider.value));
        }
    }
}