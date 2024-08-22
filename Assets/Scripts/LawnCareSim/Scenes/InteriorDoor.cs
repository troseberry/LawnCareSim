
using LawnCareSim.Events;
using LawnCareSim.Interaction;
using LawnCareSim.UI;
using UnityEngine;

namespace LawnCareSim.Scenes
{
    public class InteriorDoor : BaseInteractable
    {
        [SerializeField] private Transform _destination;
        [SerializeField] private RoomName _startRoom;
        [SerializeField] private RoomName _destRoom;

        internal Transform Destination => _destination;
        internal RoomName StartRoom => _startRoom;
        internal RoomName DestinationRoom => _destRoom;

        public override string Prompt => "Enter";

        public override void Interact()
        {
            base.Interact();

            LocationTransitionController.Instance.TransitionBetweenInteriors(this);
            ForceExit();
        }
    }
}
