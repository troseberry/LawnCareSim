using Core.GameFlow;
using Core.UI;
using LawnCareSim.Events;
using LawnCareSim.Input;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace LawnCareSim.UI
{
    public class MenuManager : MonoBehaviour, IManager
    {
        private const float EVENT_WAIT_TIME = 0.2f;

        #region Private Vars
        private Dictionary<MenuName, IMenu> _menusMap;

        private bool _gameMenusDisabled;

        private MenuName _currentMenuName;
        private IMenu _currentMenuView;

        private Timer _interactEventWaitTimer;
        private bool _waitForInteractEventTimer = false;
        #endregion

        public static MenuManager Instance;

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeManager();
        }
        #endregion

        #region IManager
        public void InitializeManager()
        {
            InputController.Instance.EscapeEvent += EscapeEventListener;
            InputController.Instance.InteractEvent += InteractEventListener;

            _interactEventWaitTimer = new Timer(EVENT_WAIT_TIME);
            _interactEventWaitTimer.Elapsed += OnEventWaitTimerElapsed;

            PopulateMenuMap();
        }
        #endregion

        #region Event Listeners
        private void OnEventWaitTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _waitForInteractEventTimer = false;
        }

        private void EscapeEventListener(object sender, EventArgs args)
        {
            CloseActiveMenu();
        }

        private void InteractEventListener(object sender, EventArgs args)
        {
            if (_waitForInteractEventTimer)
            {
                return;
            }

            if (IsInteractableMenuOpen())
            {
                CloseActiveMenu();
            }
        }
        #endregion

        #region Changing Menus
        public void OpenMenu(MenuName name, bool forceCloseExisting = false)
        {
            if (_gameMenusDisabled)
            {
                return;
            }

            if (!_menusMap.TryGetValue(name, out var menuView))
            {
                return;
            }

            if (_currentMenuName != MenuName.Invalid)
            {
                if (!forceCloseExisting)
                {
                    return;
                }

                // Trying to force open an already open menu, bail
                if (_currentMenuView == menuView)
                {
                    return;
                }

                _currentMenuView.Close();
                EventRelayer.Instance.OnMenuClosed(_currentMenuName);

                _currentMenuView = null;
                _currentMenuName = MenuName.Invalid;
            }

            _currentMenuView = menuView;
            _currentMenuName = name;

            _currentMenuView.Open();
            _interactEventWaitTimer.Start();
            _waitForInteractEventTimer = true;

            EventRelayer.Instance.OnMenuOpened(_currentMenuName);
        }

        public bool ToggleMenu(MenuName name, bool forceCloseExisting = false)
        {
            if (_gameMenusDisabled)
            {
                return false;
            }

            if (!_menusMap.TryGetValue(name, out var menuView))
            {
                return false;
            }

            if (_currentMenuView == null)
            {
                _currentMenuView = menuView;
                _currentMenuName = name;

                _currentMenuView.Open();
                _interactEventWaitTimer.Start();
                _waitForInteractEventTimer = true;

                EventRelayer.Instance.OnMenuOpened(_currentMenuName);

                return true;
            }
            else
            {
                if (_currentMenuView == menuView || forceCloseExisting)
                {
                    _currentMenuView.Close();
                    EventRelayer.Instance.OnMenuClosed(_currentMenuName);

                    _currentMenuView = null;
                    _currentMenuName = MenuName.Invalid;

                    return true;
                }
            }

            return false;
        }

        public void CloseMenu(MenuName name)
        {
            if (_gameMenusDisabled)
            {
                return;
            }

            if (!_menusMap.TryGetValue(name, out var menuView))
            {
                return;
            }

            if (_currentMenuView == menuView)
            {
                _currentMenuView.Close();
                EventRelayer.Instance.OnMenuClosed(_currentMenuName);

                _currentMenuView = null;
                _currentMenuName = MenuName.Invalid;

            }
        }

        public void CloseActiveMenu()
        {
            if (_currentMenuView == null)
            {
                return;
            }

            _currentMenuView.Close();
            EventRelayer.Instance.OnMenuClosed(_currentMenuName);

            _currentMenuView = null;
            _currentMenuName = MenuName.Invalid;
        }
        #endregion

        #region Menu Status
        public bool IsAnyMenuOpen()
        {
            return _currentMenuView != null;
        }

        public bool IsSpecificMenuOpen(MenuName name)
        {
            if (!_menusMap.TryGetValue(name, out var menuView))
            {
                Debug.LogError($"[{this}][{nameof(IsSpecificMenuOpen)}] - Checking for menu {name} but it does not exist in map.");
                return false;
            }

            return _currentMenuView == menuView;
        }

        private bool IsInteractableMenuOpen()
        {
            switch (_currentMenuName)
            {
                case MenuName.WorkTruck:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Helpers
        private void PopulateMenuMap()
        {
            _menusMap = new Dictionary<MenuName, IMenu>();

            var menus = Enum.GetValues(typeof(MenuName));
            foreach (var val in menus)
            {
                var menu = (MenuName)val;
                IMenu toAdd = null;
                switch (menu)
                {
                    case MenuName.WorkTruck:
                        toAdd = Gear.WorkTruckMenu.Instance;
                        break;
                    default:
                        break;
                }

                if (toAdd != null)
                {
                    toAdd.InitializeMenuView();
                    _menusMap.Add(menu, toAdd);
                }
            }
        }
        #endregion
    }
}
