
using LawnCareSim.Events;
using LawnCareSim.Interaction;
using LawnCareSim.UI;
using UnityEngine;

namespace LawnCareSim.Scenes
{
    public class InteriorDoor : BaseInteractable
    {
        [SerializeField] private RoomLocation _fromRoom;
        [SerializeField] private RoomLocation _toRoom;

        public override string Prompt => "Enter";

        public override void Interact()
        {
            base.Interact();

            LocationTransitionController.Instance.TransitionBetweenInteriors(_fromRoom, _toRoom);
            ForceExit();
        }
    }
}
