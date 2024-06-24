using LawnCareSim.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LawnCareSim.Input
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;
        private InputMaster InputMaster;

        public event EventHandler<Vector2> MoveEvent;
        public event EventHandler InteractEvent;
        public event EventHandler TabMenuEvent;
        public event EventHandler EscapeEvent;

        public event EventHandler DebugMenuEvent;

        void Awake()
        {
            Instance = this;
            InitInputMaster();
        }

        private void InitInputMaster()
        {
            InputMaster = new InputMaster();
            InputMaster.General.Enable();
            InputMaster.MouseKeyboard.Enable();
            InputMaster.UI.Enable();
            InputMaster.Debug.Enable();

            InputMaster.General.Escape.performed += Escape;

            InputMaster.MouseKeyboard.Move.performed += Move;
            InputMaster.MouseKeyboard.Interact.performed += Interact;
            InputMaster.MouseKeyboard.TabMenu.performed += TabMenu;

            InputMaster.Debug.DebugMenu.performed += DebugMenu;
        }

        
        public void EnableMenuInput(MenuName name, bool disableGameInput = true)
        {
            switch (name)
            {
                case MenuName.Test:
                    //InputMaster.CraftingMenu.Enable();
                    break;
            }

            if (disableGameInput)
            {
                InputMaster.MouseKeyboard.Disable();
            }
        }

        public void DisableMenuInput(MenuName name, bool enableGameInput = true)
        {
            switch (name)
            {
                case MenuName.Test:
                    //InputMaster.CraftingMenu.Disable();
                    break;
            }

            if (enableGameInput)
            {
                InputMaster.MouseKeyboard.Enable();
            }
        }

        #region General
        private void Escape(InputAction.CallbackContext obj)
        {
            EscapeEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Mouse & Keyboard
        private void Move(InputAction.CallbackContext obj)
        {
            MoveEvent?.Invoke(this, obj.ReadValue<Vector2>());

        }

        private void Interact(InputAction.CallbackContext obj)
        {
            InteractEvent?.Invoke(this, EventArgs.Empty);
        }

        private void TabMenu(InputAction.CallbackContext obj)
        {
            TabMenuEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Debug
        private void DebugMenu(InputAction.CallbackContext obj)
        {
            DebugMenuEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
