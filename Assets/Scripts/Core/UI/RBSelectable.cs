using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
    public class RBSelectable : Selectable
    {
        public EventTriggerType EventTriggerType = EventTriggerType.Select;
        public bool AutoDeselect = true;
        public List<SelectableAction> Actions = new List<SelectableAction>();

        private UnityEvent OnSelectedEvent = new UnityEvent();
        private UnityEvent OnDeselectedEvent = new UnityEvent();

        private bool _selected;

        protected override void Awake()
        {
            transition = Transition.None;
            foreach (var action in Actions)
            {
                action.SetupTargetElements();
            }

            base.Awake();
        }

        public void AddOnSelectedListener(UnityAction call)
        {
            //Debug.Log($"RBSelectable add selection action {name}");
            OnSelectedEvent.AddListener(call);
        }

        public void AddOnDeselectedListener(UnityAction call)
        {
            OnDeselectedEvent.AddListener(call);
        }

        

        public override void OnSelect(BaseEventData eventData)
        {
            if (_selected)
            {
                return;
            }

            if (EventTriggerType != EventTriggerType.Select)
            {
                base.OnSelect(eventData);
                return;
            }

            _selected = true;

            RunSelectedActions();
            OnSelectedEvent.Invoke();

            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (!_selected)
            {
                return;
            }

            if (EventTriggerType != EventTriggerType.Select)
            {
                base.OnDeselect(eventData);
                return;
            }

            if (!AutoDeselect) return;
            _selected = false;

            RunDeselectedActions();
            OnDeselectedEvent.Invoke();

            base.OnDeselect(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (EventTriggerType != EventTriggerType.PointerDown)
            {
                base.OnPointerDown(eventData);
                return;
            }

            _selected = !_selected;

            if (_selected)
            {
                RunSelectedActions();
                OnSelectedEvent.Invoke();
            }
            else
            {
                RunDeselectedActions();
                OnDeselectedEvent.Invoke();
            }

            base.OnPointerDown(eventData);
        }

        private void RunSelectedActions()
        {
            foreach (var action in Actions)
            {
                action.PerformSelection();
            }
        }

        private void RunDeselectedActions()
        {
            foreach (var action in Actions)
            {
                action.PerformDeselection();
            }
        }

        public void ManuallyRunDeselectionActions()
        {
            _selected = false;
            RunDeselectedActions();
        }

        public void ManuallyRunSelectionActions()
        {
            _selected = true;
            RunSelectedActions();
        }
    }
}