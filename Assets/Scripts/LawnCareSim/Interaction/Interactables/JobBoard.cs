using LawnCareSim.UI;
using System;

namespace LawnCareSim.Interaction
{
    public class JobBoard : BaseInteractable
    {
        #region IInteractable
        public override string Prompt => "Job Board";

        public override void Interact()
        {
            base.Interact();

            MenuManager.Instance.OpenMenu(MenuName.JobBoard);
        }
        #endregion
    }
}
