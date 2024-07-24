using Core.UI;
using UnityEngine;

namespace LawnCareSim.UI
{
    public class BaseMenuUIComponent : MonoBehaviour, IMenuUIComponent
    {
        public virtual object BackingData { get; set; }

        public virtual void Clear(bool clearBackingData)
        {
            if (clearBackingData)
            {
                BackingData = default;
            }
        }

        public virtual void UpdateInterface()
        {
            
        }
    }
}
