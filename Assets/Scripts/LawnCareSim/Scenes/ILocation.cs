using UnityEngine;

namespace LawnCareSim.Scenes
{
    public interface ILocation
    {
        public LocationType LocationType { get; }

        public Transform TransitionDestination { get; }
    }
}
