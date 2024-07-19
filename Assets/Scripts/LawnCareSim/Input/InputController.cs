using LawnCareSim.Gear;
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

        public event EventHandler DebugMenuEvent;

        public event EventHandler EscapeEvent;

        public event EventHandler<Vector2> MoveEvent;
        public event EventHandler InteractEvent;
        public event EventHandler TabMenuEvent;

        public event EventHandler<float> AdjustedCutHeightEvent;

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
            InputMaster.Mower.Disable();

            InputMaster.Debug.DebugMenu.performed += DebugMenu;

            InputMaster.General.Escape.performed += Escape;

            InputMaster.MouseKeyboard.Move.performed += Move;
            InputMaster.MouseKeyboard.Interact.performed += Interact;
            InputMaster.MouseKeyboard.TabMenu.performed += TabMenu;

            InputMaster.Mower.AdjustCutHeight.performed += AdjustCutHeight;
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

        #region Debug
        private void DebugMenu(InputAction.CallbackContext obj)
        {
            DebugMenuEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

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

        #region Gear
        public void EnableGearInput(GearType gearType)
        {
            switch(gearType)
            {
                case GearType.Mower:
                    InputMaster.Mower.Enable();
                    break;
                default:
                    break;
            }
        }

        public void DisableGearInput(GearType gearType)
        {
            switch (gearType)
            {
                case GearType.Mower:
                    InputMaster.Mower.Disable();
                    break;
                default:
                    break;
            }
        }

        private void AdjustCutHeight(InputAction.CallbackContext obj)
        {
            AdjustedCutHeightEvent?.Invoke(this, obj.ReadValue<float>());
        }
        #endregion
    }
}
