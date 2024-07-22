using UnityEngine;

namespace LawnCareSim.Interaction
{
    public class BaseInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Canvas _interactionPromptCanvas;

        private const string PlAYER_TAG = "Player";

        #region Unity Methods
        private void Start()
        {
            _interactionPromptCanvas.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == PlAYER_TAG)
            {
                _interactionPromptCanvas.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == PlAYER_TAG)
            {
                _interactionPromptCanvas.enabled = false;
            }
        }
        #endregion

        #region IInteractable
        public virtual string Prompt => "Interact";

        public virtual bool CanInteract()
        {
            return true;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Interact()
        {

        }
        #endregion
    }
}
