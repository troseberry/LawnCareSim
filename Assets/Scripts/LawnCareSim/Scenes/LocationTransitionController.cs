using LawnCareSim.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Awake()
        {
            Instance = this;
        }

        //on scene load, get Location objects in scene
        // Is this needed? If all doors just make the call to transition this controller
        // shouldn't need any references


        internal void TransitionBetweenInteriors(RoomLocation fromRoom, RoomLocation toRoom)
        {
            StartCoroutine(Transition(fromRoom, toRoom));
        }

        private IEnumerator Transition(RoomLocation fromRoom, RoomLocation toRoom)
        {
            EventRelayer.Instance.OnDisablePlayerControl(true);

            yield return new WaitForSecondsRealtime(0.1f);

            toRoom.RoomGroup.SetActive(true);

            yield return new WaitForSecondsRealtime(0.1f);

            fromRoom.RoomGroup.SetActive(false);

            yield return new WaitForSecondsRealtime(0.1f);

            EventRelayer.Instance.OnMovePlayer(fromRoom.TransitionDestination);

            yield return new WaitForSecondsRealtime(0.1f);

            EventRelayer.Instance.OnDisablePlayerControl(false);
        }
    }
}
