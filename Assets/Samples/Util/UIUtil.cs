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
}