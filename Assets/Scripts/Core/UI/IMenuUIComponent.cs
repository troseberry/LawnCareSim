using System;

namespace Core.UI
{
    interface IMenuUIComponent
    {
        object BackingData { get; set; }

        void UpdateInterface();

        void Clear(bool clearBackingData);
    }
}
