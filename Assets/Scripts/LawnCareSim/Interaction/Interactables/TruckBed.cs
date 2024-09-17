using LawnCareSim.Scenes;
using LawnCareSim.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LawnCareSim.Interaction
{
    public class TruckBed : BaseInteractable
    {
        #region IInteractable

        // switch based on work scene or garage scene
        // Change Gear in work scene
        // Load up gear in garage scene
        public override string Prompt => "Manage Gear";

        public override void Interact()
        {
            base.Interact();

            SceneName currentScene = (SceneName)SceneManager.GetActiveScene().buildIndex;

            switch (currentScene)
            {
                case SceneName.Work:
                    MenuManager.Instance.OpenMenu(MenuName.WorkTruck);
                    break;
                case SceneName.Interior:
                    break;
            }

        }
        #endregion
    }
}