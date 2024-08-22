using LawnCareSim.Events;
using System.Collections;
using UnityEngine;

namespace LawnCareSim.Scenes
{
    internal enum RoomName
    {
        Invalid,
        Office,
        Garage
    }

    // ability to move between rooms but also scenes
    // via interactables. like door in office
    public class LocationTransitionController : MonoBehaviour
    {
        internal static LocationTransitionController Instance;

        [SerializeField] private GameObject _officeRoom;
        [SerializeField] private GameObject _garageRoom;

        private void Awake()
        {
            Instance = this;
        }

        internal void TransitionBetweenInteriors(InteriorDoor door)
        {
            StartCoroutine(Transition(door));
        }

        private IEnumerator Transition(InteriorDoor door)
        {
            EventRelayer.Instance.OnDisablePlayerControl(true);

            yield return new WaitForSecondsRealtime(0.1f);

            GetRoomFromName(door.DestinationRoom).SetActive(true);

            yield return new WaitForSecondsRealtime(0.1f);

            GetRoomFromName(door.StartRoom).SetActive(false);

            yield return new WaitForSecondsRealtime(0.1f);

            EventRelayer.Instance.OnMovePlayer(door.Destination);

            yield return new WaitForSecondsRealtime(0.1f);

            EventRelayer.Instance.OnDisablePlayerControl(false);
        }

        private GameObject GetRoomFromName(RoomName name)
        {
            switch(name)
            {
                case RoomName.Office:
                    return _officeRoom;
                case RoomName.Garage:
                    return _garageRoom;
            }

            return null;
        }
    }
}
