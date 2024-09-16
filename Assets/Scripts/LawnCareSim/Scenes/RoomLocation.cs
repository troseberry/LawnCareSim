using UnityEngine;

namespace LawnCareSim.Scenes
{
    public class RoomLocation : BaseLocation
    {
        public override LocationType LocationType => LocationType.Room;

        public GameObject RoomGroup => _roomGroup;

        [SerializeField] private GameObject _roomGroup;
    }
}
