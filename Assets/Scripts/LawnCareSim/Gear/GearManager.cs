using Core.GameFlow;
using LawnCareSim.Events;
using LawnCareSim.Input;
using System;
using UnityEngine;

namespace LawnCareSim.Gear
{
    public partial class GearManager : MonoBehaviour
    {
        public static GearManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeManager();
        }

        public void InitializeManager()
        {
            _mowerGear = _lawnMowerGO.GetComponent<IGear>();
            _edgerGear = _edgerGO.GetComponent<IGear>();
            _striperGear = _striperGO.GetComponent<IGear>();

            InputController.Instance.InteractEvent += InteractEventListener;
        }

        #region Event Listeners
        private void InteractEventListener(object sender, EventArgs args)
        {
            if (_equippedGear != null)
            {
                _equippedGear.TogglePower();
            }
        }
        #endregion
    }

    public partial class GearManager : IManager
    {
        private GearType _equippedGearType = GearType.None;
        private IGear _equippedGear;
        private GameObject _equippedGearGO;

        private IGear _mowerGear;
        private IGear _edgerGear;
        private IGear _striperGear;

        [SerializeField] private GameObject _lawnMowerGO;
        [SerializeField] private GameObject _edgerGO;
        [SerializeField] private GameObject _striperGO;

        private void SwitchGear(GearType newGear)
        {
            if (newGear == _equippedGearType)
            {
                return;
            }

            _equippedGear?.TurnOff();
            if (_equippedGearGO != null) _equippedGearGO.SetActive(false);

            switch (newGear)
            {
                case GearType.Mower:
                    _equippedGear = _mowerGear;
                    _equippedGearGO = _lawnMowerGO;
                    break;
                case GearType.Edger:
                    _equippedGear = _edgerGear;
                    _equippedGearGO = _edgerGO;
                    break;
                case GearType.Striper:
                    _equippedGear = _striperGear;
                    _equippedGearGO = _striperGO;
                    break;
                default:
                    _equippedGear = null;
                    _equippedGearGO = null;
                    break;
            }

            _equippedGearType = newGear;
            if (_equippedGearGO != null) _equippedGearGO.SetActive(true);
            EventRelayer.Instance.OnGearSwitched(_equippedGearType);
        }
    }

    #region Debug
    public partial class GearManager
    {
        private void OnGUI()
        {
            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            // Equipped Gear
            Rect mainRect = new Rect(width * 0.82f, height * 0.04f, 300, 200);
            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Equipped Gear: {_equippedGear?.GearType}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Powered On: {_equippedGear?.IsActive}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 60, 250, 30), $"Energy {_equippedGear?.Energy}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 90, 250, 30), $"Durability: {_equippedGear?.Durability}", fontStyle);

            // Gear Switching
            Rect bottomRect = new Rect(width * 0.5f - 450, height * 0.8f, 900, 200);
            GUI.Box(bottomRect, GUIContent.none);
            if (GUI.Button(new Rect(bottomRect.x + 15, bottomRect.y + 25, 150, 150), "None"))
            {
                SwitchGear(GearType.None);
            }

            if (GUI.Button(new Rect(bottomRect.x + 180, bottomRect.y + 25, 150, 150), "Mower"))
            {
                SwitchGear(GearType.Mower);
            }

            if (GUI.Button(new Rect(bottomRect.x + 345, bottomRect.y + 25, 150, 150), "Edger"))
            {
                SwitchGear(GearType.Edger);
            }

            if (GUI.Button(new Rect(bottomRect.x + 510, bottomRect.y + 25, 150, 150), "Striper"))
            {
                SwitchGear(GearType.Striper);
            }
        }
    }
    #endregion
}
