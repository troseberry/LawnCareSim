using Core.UI;
using UnityEngine;

namespace LawnCareSim.UI
{
    public class BaseMenu : MonoBehaviour, IMenu
    {
        protected MenuState _state;
        protected Canvas _canvas;

        public MenuState State
        {
            get => _state;
            set => _state = value;
        }

        public Canvas MenuCanvas => _canvas;

        public virtual void InitializeMenuView()
        {
            _canvas = GetComponent<Canvas>();

            _canvas.enabled = false;
            _state = MenuState.Closed;
        }

        public virtual void Open()
        {
            _canvas.enabled = true;
            _state = MenuState.Open;
        }

        public virtual void Close()
        {
            _canvas.enabled = false;
            _state = MenuState.Closed;
        }
    }
}
