using LawnCareSim.Events;
using LawnCareSim.Player;
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

        internal void TransitionBetweenScenes(SceneName fromScene, SceneName toScene)
        {
            StartCoroutine(Transition(fromScene, toScene));
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

        private IEnumerator Transition(SceneName fromScene, SceneName toScene)
        {
            EventRelayer.Instance.OnDisablePlayerControl(true);

            yield return new WaitForSecondsRealtime(0.1f);

            yield return SceneManager.LoadSceneAsync((int)toScene, LoadSceneMode.Additive);

            Scene loadedScene = SceneManager.GetSceneByBuildIndex((int)toScene);

            SceneManager.MoveGameObjectToScene(PlayerRef.Instance.gameObject, loadedScene);

            // To-Do: better way to do this other than GameObject.Find? Scripts for spawn locations so I can get quick references
            var spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null)
            {
                EventRelayer.Instance.OnMovePlayer(spawn.transform);
            }

            SceneManager.UnloadSceneAsync((int)fromScene);

            yield return new WaitForSecondsRealtime(0.1f);

            EventRelayer.Instance.OnDisablePlayerControl(false);
        }
    }
}
