using LawnCareSim.Scenes;
using UnityEngine.SceneManagement;

namespace LawnCareSim.Interaction
{
    public class TruckDoor : BaseInteractable
    {
        // switch based on work scene or garage scene
        // Start Job in garage scene
        // Exit job in work scene

        public override string Prompt => "Use Truck";

        public override void Interact()
        {
            base.Interact();

            SceneName currentScene = (SceneName)SceneManager.GetActiveScene().buildIndex;
            
            switch(currentScene)
            {
                case SceneName.Work:
                    break;
                case SceneName.Interior:
                    break;
            }
        }
    }
}
