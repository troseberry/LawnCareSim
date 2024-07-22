using UnityEngine;


namespace LawnCareSim.Interaction
{
    public class WorkTruck : BaseInteractable
    {
        #region IInteractable
        public override string Prompt => "Change Gear";
        #endregion
    }
}