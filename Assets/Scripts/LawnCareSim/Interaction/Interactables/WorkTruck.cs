using LawnCareSim.UI;

namespace LawnCareSim.Interaction
{
    public class WorkTruck : BaseInteractable
    {
        #region IInteractable
        public override string Prompt => "Change Gear";

        public override void Interact()
        {
            base.Interact();

            MenuManager.Instance.OpenMenu(MenuName.WorkTruck);
        }
        #endregion
    }
}