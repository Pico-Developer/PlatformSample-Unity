using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace PICO.Platform.Samples.Util
{
    /*
     * If using PICO device,you should use `XR UI Input Module` in the EventSystem
     * game object.
     * If using PC, you should use `Input system UI Input Module`.
     * This script will switch the event system automatically.
     */
    public class EventSystemSwitcher : MonoBehaviour
    {
        void handleCanvas()
        {
            //处理Canvas，让所有的Canvas都添加TrackDevice
            var canvas = FindObjectsOfType<Canvas>();
            foreach (var c in canvas)
            {
                var g = c.GetComponent<TrackedDeviceGraphicRaycaster>();
                if (g == null)
                {
                    c.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                }
            }
        }

        private void Start()
        {
            handleCanvas();
            var eventSystem = FindObjectOfType<EventSystem>();
            var xrUiInputModule = eventSystem.GetComponent<XRUIInputModule>();
            var uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (Application.platform == RuntimePlatform.Android)
            {
                if (xrUiInputModule == null)
                {
                    return;
                }

                xrUiInputModule.enabled = true;
                if (uiInputModule != null)
                {
                    uiInputModule.enabled = false;
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.isEditor)
            {
                if (uiInputModule == null)
                {
                    uiInputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                    uiInputModule.enabled = true;
                    xrUiInputModule.enabled = false;
                }
            }
        }
    }
}