using LawnCareSim.UI;

namespace LawnCareSim.Interaction
{
    public class WorkTruck : BaseInteractable
    {
        #region IInteractable

        // switch based on work scene or garage scene
        // Change Gear at work scene
        // Start Job in garage scene
        public override string Prompt => "Change Gear";

        public override void Interact()
        {
            base.Interact();

            MenuManager.Instance.OpenMenu(MenuName.WorkTruck);
        }
        #endregion
    }
}