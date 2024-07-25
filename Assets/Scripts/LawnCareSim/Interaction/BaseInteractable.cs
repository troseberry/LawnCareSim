using LawnCareSim.Events;
using UnityEngine;

namespace LawnCareSim.Interaction
{
    public class BaseInteractable : MonoBehaviour, IInteractable
    {
        private EventRelayer _eventRelayer;

        private const string PLAYER_TAG = "Player";
        private bool _isInInteractionZone = false;

        #region Unity Methods
        private void Start()
        {
            _eventRelayer = EventRelayer.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == PLAYER_TAG)
            {
                _isInInteractionZone = true;
                _eventRelayer.OnEnteredInteractionZone(Prompt);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == PLAYER_TAG)
            {
                _isInInteractionZone = false;
                EventRelayer.Instance.OnExitedInteractionZone();
            }
        }
        #endregion

        #region IInteractable
        public virtual string Prompt => "Interact";

        public virtual bool CanInteract()
        {
            return _isInInteractionZone;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Interact()
        {
            if (!CanInteract())
            {
                return;
            }
        }
        #endregion
    }
}
