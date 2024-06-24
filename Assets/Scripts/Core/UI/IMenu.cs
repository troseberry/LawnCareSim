using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.UI
{
    public enum MenuState { Invalid, Open, Closed }

    public interface IMenu
    {
        MenuState State { get; set; }

        //bool CanManuallyClose { get; }

        Canvas MenuCanvas { get; }

        void Open();

        void Close();

       void InitializeMenuView();
    }
}
