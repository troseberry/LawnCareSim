using Core.Utility;
using LawnCareSim.Events;
using LawnCareSim.Interaction;
using TMPro;
using UnityEngine;

namespace LawnCareSim.UI
{
    public class HUD : MonoBehaviour
    {
        private Canvas _canvas;

        [SerializeField] private GameObject _interactionGroup;
        private TextMeshProUGUI _interactPromptText;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();

            EventRelayer.Instance.EnteredInteractionZoneEvent += EnteredInteractionZoneEventListener;
            EventRelayer.Instance.ExitedInteractionZoneEvent += ExitedInteractionZoneEventListener;

            UIHelpers.SetUpUIElement(_interactionGroup.transform, ref _interactPromptText, "InteractPromptText");

            _interactionGroup.SetActive(false);
        }

        #region Event Listeners
        private void EnteredInteractionZoneEventListener(object sender, (IInteractable, string) args)
        {
            ToggleInteractionPrompt(true, args.Item2);
        }

        private void ExitedInteractionZoneEventListener(object sender, IInteractable args)
        {
            ToggleInteractionPrompt(false);
        }
        #endregion

        #region Interaction
        private void ToggleInteractionPrompt(bool state, string promptText = "")
        {
            _interactionGroup.SetActive(state);
            _interactPromptText.text = promptText;
        }
        #endregion
    }
}
