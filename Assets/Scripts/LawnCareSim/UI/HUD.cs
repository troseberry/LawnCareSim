using Core.Utility;
using LawnCareSim.Events;
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
        private void EnteredInteractionZoneEventListener(object sender, string args)
        {
            ToggleInteractionPrompt(true, args);
        }

        private void ExitedInteractionZoneEventListener(object sender, System.EventArgs args)
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
