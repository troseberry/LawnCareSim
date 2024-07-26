using Core.GameFlow;
using LawnCareSim.Events;
using LawnCareSim.Input;
using System;
using UnityEngine;

namespace LawnCareSim.Interaction
{
    public class InteractionManager : MonoBehaviour, IManager
    {
        public static InteractionManager Instance;

        private IInteractable _currentInteractable;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeManager();
        }

        public void InitializeManager()
        {
            InputController.Instance.InteractEvent += InteractEventListener;

            EventRelayer.Instance.EnteredInteractionZoneEvent += EnteredInteractionZoneEventListener;
            EventRelayer.Instance.ExitedInteractionZoneEvent += ExitedInteractionZoneEventListener;
        }

        #region Event Listeners
        private void EnteredInteractionZoneEventListener(object sender, (IInteractable, string) args)
        {
            ChangeInteractable(args.Item1, true);
        }

        private void ExitedInteractionZoneEventListener(object sender, IInteractable args)
        {
            ChangeInteractable(args, false);
        }

        private void InteractEventListener(object sender, EventArgs args)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
            }
        }
        #endregion

        private void ChangeInteractable(IInteractable interactable, bool entered)
        {
            bool interactablesAreTheSame = _currentInteractable == interactable;

            if (entered && !interactablesAreTheSame)
            {
                _currentInteractable = interactable;
            }
            else if (!entered && interactablesAreTheSame)
            {
                _currentInteractable = null;
            }
        }
    }
}
