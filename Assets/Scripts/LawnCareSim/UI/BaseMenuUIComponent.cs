using Core.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

namespace LawnCareSim.UI
{
    public class BaseMenuUIComponent : MonoBehaviour, IMenuUIComponent
    {
        private EventTrigger _eventTrigger;

        private void Start()
        {
            Initialize();
        }

        public virtual object BackingData { get; set; }

        public virtual void Initialize()
        {
            _eventTrigger = GetComponent<EventTrigger>();

            AddEventListener(EventTriggerType.PointerEnter, (data) => OnHover(true));
            AddEventListener(EventTriggerType.PointerExit, (data) => OnHover(false));
        }

        public virtual void Clear(bool clearBackingData = false)
        {
            if (clearBackingData)
            {
                BackingData = default;
            }
        }

        public virtual void UpdateInterface()
        {
            
        }

        public void AddEventListener(EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            if (_eventTrigger == null)
            {
                Debug.LogError($"[{this}][AddEventEntry] - No event trigger component found on gameobject {name}");
                return;
            }

            Entry entry = _eventTrigger.triggers.Find(entry => entry.eventID == triggerType);

            if (entry == null)
            {
                entry = new Entry { eventID = triggerType };
                entry.callback.AddListener(action);
                _eventTrigger.triggers.Add(entry);
            }
            else
            {
                int existingIndex = _eventTrigger.triggers.IndexOf(entry);
                _eventTrigger.triggers[existingIndex].callback.AddListener(action);
            }
        }

        public virtual void OnHover(bool start)
        {

        }
    }
}
