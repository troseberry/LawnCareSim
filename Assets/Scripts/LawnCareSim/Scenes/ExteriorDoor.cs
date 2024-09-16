using LawnCareSim.Interaction;
using UnityEngine;

namespace LawnCareSim.Scenes
{
    public class ExteriorDoor : BaseInteractable
    {
        [SerializeField] private SceneName _fromScene;
        [SerializeField] private SceneName _toScene;

        public override string Prompt => "Use Door";

        public override void Interact()
        {
            base.Interact();

            LocationTransitionController.Instance.TransitionBetweenScenes(_fromScene, _toScene);
            ForceExit();
        }
    }
}
