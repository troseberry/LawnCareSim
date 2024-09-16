using UnityEngine;
using UnityEngine.SceneManagement;

namespace LawnCareSim.Scenes
{
    public class SceneLocation : BaseLocation
    {
        public override LocationType LocationType => LocationType.Scene;

        [SerializeField] private SceneName _locationScene;

        public SceneName LocationScene => _locationScene;
    }
}
