using UnityEngine;

namespace LawnCareSim.Player
{
    public class PlayerRef : MonoBehaviour
    {
        public static PlayerRef Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}
