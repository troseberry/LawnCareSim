using Core.GameFlow;
using LawnCareSim.Data;
using LawnCareSim.Events;
using LawnCareSim.Input;
using LawnCareSim.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace LawnCareSim.Gear
{
    public partial class GearManager : MonoBehaviour
    {
        public static GearManager Instance;

        [SerializeField] private Transform _gearSpawn;

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
            _inputController = InputController.Instance;
            _inputController.InteractEvent += InteractEventListener;

            CreateDebugGearList();
        }

        #region Event Listeners
        private void InteractEventListener(object sender, EventArgs args)
        {
            if (_equippedGear.IGear != null)
            {
                _equippedGear.IGear.TogglePower();
            }
        }
        #endregion
    }

    public partial class GearManager : IManager
    {
        private InputController _inputController;

        private Dictionary<GearType, RuntimeGearData> _truckGear;
        private EquippedGear _equippedGear = new EquippedGear();

        private void SwitchGear(GearType newGear)
        {
            if (newGear == _equippedGear.GearType)
            {
                return;
            }

            _equippedGear.IGear?.TurnOff();
            _equippedGear.GameObject?.SetActive(false);

            if (!GetGear(newGear, out _equippedGear))
            {
                return;
            }

            HandleGearInputChange(_equippedGear.GearType, newGear);

            _equippedGear.GearType = newGear;
            _equippedGear.GameObject?.SetActive(true);

            EventRelayer.Instance.OnGearSwitched(_equippedGear.GearType);
        }

        private void HandleGearInputChange(GearType prevGear, GearType newGear)
        {
            _inputController.DisableGearInput(prevGear);
            _inputController.EnableGearInput(newGear);
        }

        private bool GetGear(GearType gearType, out EquippedGear gear)
        {
            gear = new EquippedGear();

            if (gearType == GearType.None)
            {
                return true;
            }

            if (!_truckGear.TryGetValue(gearType, out var gearEntry))
            {
                Debug.LogError($"[GearManager][GetGear] - Trying to get gear of type {gearType} but it is not present in loaded gear.");
                return false;
            }

            // if either is null, try spawn again
            if (gearEntry.GearScriptRef == null || gearEntry.GearGameObject == null)
            {
                gearEntry.GearGameObject = Instantiate(gearEntry.GearData.Prefab, _gearSpawn);

                var parentConstraint = new ConstraintSource { sourceTransform = PlayerRef.Instance.transform, weight = 1.0f };
                gearEntry.GearGameObject.GetComponent<ParentConstraint>().SetSource(0, parentConstraint);

                gearEntry.GearScriptRef = gearEntry.GearGameObject.GetComponent<IGear>();
                _truckGear[gearType] = gearEntry;
            }

            gear = new EquippedGear { GearType = gearType, IGear = gearEntry.GearScriptRef, GameObject = gearEntry.GearGameObject };
            return true;
        }

        public List<GearInfo> GetAllGearInfo()
        {
            List<GearInfo> gearList = new List<GearInfo>();

            foreach(var entry in _truckGear)
            {
                gearList.Add(entry.Value.GearData);
            }

            return gearList;
        }
    }

    #region Debug
    public partial class GearManager
    {
        public bool ShowDebugGUI;

        private void CreateDebugGearList()
        {
            _truckGear = new Dictionary<GearType, RuntimeGearData>();

            var dataManager = MasterDataManager.Instance.GearDataManager;
            if (dataManager.GetGearData(GearVariant.FuelPushMower, out var mowerData))
            {
                _truckGear.Add(GearType.Mower, new RuntimeGearData(mowerData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.FuelEdger, out var edgerData))
            {
                _truckGear.Add(GearType.Edger, new RuntimeGearData(edgerData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.ManualPushStriper, out var striperData))
            {
                _truckGear.Add(GearType.Striper, new RuntimeGearData(striperData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.FuelVacuum, out var vacuumData))
            {
                _truckGear.Add(GearType.Vacuum, new RuntimeGearData(vacuumData, null, null));
            }
        }

        private void OnGUI()
        {
            if (!ShowDebugGUI)
            {
                return;
            }

            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            // Equipped Gear
            Rect mainRect = new Rect(width * 0.82f, height * 0.04f, 300, 200);
            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Equipped Gear: {_equippedGear.IGear?.GearType}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Powered On: {_equippedGear.IGear?.IsActive}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 60, 250, 30), $"Energy {_equippedGear.IGear?.Energy}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 90, 250, 30), $"Durability: {_equippedGear.IGear?.Durability}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 120, 300, 30), $"Stats: {_equippedGear.IGear?.DebugUnuiqueStats()}", fontStyle);

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

            if (GUI.Button(new Rect(bottomRect.x + 675, bottomRect.y + 25, 150, 150), "Vacuum"))
            {
                SwitchGear(GearType.Vacuum);
            }
        }
    }
    #endregion
}
