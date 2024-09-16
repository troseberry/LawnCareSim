using UnityEngine;

namespace LawnCareSim.Scenes
{
    public enum LocationType
    {
        Invalid,
        Scene,
        Room
    }

    public class BaseLocation : MonoBehaviour, ILocation
    {
        public virtual LocationType LocationType => LocationType.Invalid;

        public virtual Transform TransitionDestination => _transitionDestination;

        [SerializeField] protected Transform _transitionDestination;
    }
}
