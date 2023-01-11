using System;
using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PICO.Platform.Samples.Invite
{
    public class LaunchDetailsCanvas : MonoBehaviour
    {
        public Text changeTime;
        public Text detail;
        public Text currentTime;

        public void SetLaunchDetail(LaunchDetails launchDetails)
        {
            changeTime.text = DateTime.Now.ToString();
            detail.text = JsonConvert.SerializeObject(launchDetails, Formatting.Indented);
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                currentTime.text = DateTime.Now.ToString();
            }
        }
    }
}