using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Pico.Platform.Samples
{
    public static class UIBehaviourExtension
    {
        public static T OnEventTriggerEvent<T>(this T self, EventTriggerType type, UnityAction<BaseEventData> action) where T : UIBehaviour
        {
            var eventTrigger = self.GetComponent<EventTrigger>() ?? self.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry {eventID = type};
            entry.callback.AddListener(action);
            eventTrigger.triggers.Add(entry);
            return self;
        }
    }

    public static class UIUtil
    {
        public static GameObject FindChild(GameObject parent, string childName)
        {
            for (var ind = 0; ind < parent.transform.childCount; ind++)
            {
                var i = parent.transform.GetChild(ind);
                if (i.gameObject.name.Trim().Equals(childName.Trim()))
                {
                    return i.gameObject;
                }
            }

            return null;
        }
        public static UnityAction<T> Debounce<T>(UnityAction<T> act, float t)
        {
            float lastRun = 0;
            return x =>
            {
                if (Time.realtimeSinceStartup - lastRun > t)
                {
                    act(x);
                    lastRun = Time.realtimeSinceStartup;
                }
            };
        }
    }
}