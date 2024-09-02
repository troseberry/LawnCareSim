using Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LawnCareSim.UI
{
    public class BaseHUD : MonoBehaviour, IHUD
    {
        protected Canvas _canvas;

        public virtual void InitializeHud()
        {
            _canvas = GetComponent<Canvas>();
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }

        public virtual void Show()
        {
            _canvas.enabled = true;
        }
    }
}
