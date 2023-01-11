using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public class Toast : MonoBehaviour
    {
        public Text text;
        private float showTime = 2;
        public float ttlSeconds = 3;

        private void Start()
        {
        }

        public void Show(string content)
        {
            showTime = Time.realtimeSinceStartup;
            text.text = content;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - showTime > ttlSeconds)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}