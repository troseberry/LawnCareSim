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
    // TO-DO: maybe rename to TruckGearManager. Anticipating there will be another manager for gear when not on a job. Like in the garage
    // Or keep all gear stuff to one class
    public partial class GearManager : MonoBehaviour
    {
        public static GearManager Instance;

        private Transform _gearSpawn;
        private Transform _workTruckPlayerSpawn;

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

            _gearSpawn = GameObject.Find("GearSpawn").transform;
            _workTruckPlayerSpawn = GameObject.Find("WorkTruckPlayerSpawn").transform;

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
        private RuntimeGearData _equippedGear = new RuntimeGearData();

        /// <summary>
        /// Change currently equipped gear.
        /// </summary>
        /// <param name="newGear"></param>
        /// <param name="unequippSameSelection">Will unequipp currently selected gear if newGear is the same as already equipped</param>
        internal void SwitchGear(GearType newGear, bool unequippSameSelection = false)
        {
            if (newGear == _equippedGear.GearType)
            {
                if (!unequippSameSelection)
                {
                    return;
                }

                newGear = GearType.None;
            }

            _equippedGear.IGear?.TurnOff();
            _equippedGear.GameObject?.SetActive(false);

            bool emptyHands = newGear == GearType.None;
            bool result = _truckGear.TryGetValue(newGear, out var foundGear);

            if (!emptyHands && !result)
            {
                Debug.LogError($"[{this}][{nameof(SwitchGear)}] - Trying to switch gear to type {newGear} but it does not exist in truck gear.");
                return;
            }

            HandleGearInputChange(_equippedGear.GearType, newGear);
            ChangeEquippedGearData(foundGear, emptyHands);

            EventRelayer.Instance.OnGearSwitched(_equippedGear.GearType);
        }

        private void ChangeEquippedGearData(RuntimeGearData newData, bool emptyHands)
        {
            if (emptyHands)
            {
                _equippedGear.GearType = GearType.None;
                _equippedGear.GearInfo = default;
                _equippedGear.IGear = null;
                _equippedGear.GameObject = null;
            }
            else
            {
                _equippedGear = newData;
                _equippedGear.GearType = newData.GearType;
                _equippedGear.GameObject?.SetActive(true);
            }
        }

        private void HandleGearInputChange(GearType prevGear, GearType newGear)
        {
            _inputController.DisableGearInput(prevGear);
            _inputController.EnableGearInput(newGear);
        }

        private void SpawnGear()
        {
            List<RuntimeGearData> newEntries = new List<RuntimeGearData>();
            foreach(var entry in _truckGear)
            {
                var gearEntry = entry.Value;

                gearEntry.GameObject = Instantiate(gearEntry.GearInfo.Prefab, _gearSpawn);

                var parentConstraint = new ConstraintSource { sourceTransform = PlayerRef.Instance.transform, weight = 1.0f };
                gearEntry.GameObject.GetComponent<ParentConstraint>().SetSource(0, parentConstraint);

                gearEntry.IGear = gearEntry.GameObject.GetComponent<IGear>();
                gearEntry.GameObject.SetActive(false);

                newEntries.Add(gearEntry);
            }

            foreach(var gear in newEntries)
            {
                gear.IGear.Initialize(GearHelpers.GetEnergyTypeForVariant(gear.GearInfo.Variant));
                _truckGear[gear.GearType] = gear;
            }
        }

        public List<GearInfo> GetAllGearInfo()
        {
            List<GearInfo> gearList = new List<GearInfo>();

            foreach(var entry in _truckGear)
            {
                gearList.Add(entry.Value.GearInfo);
            }

            return gearList;
        }

        public bool GetGearStatsForType(GearType type, out List<GearStat> stats)
        {
            stats = null;

            if (!_truckGear.TryGetValue(type, out var data))
            {
                return false;
            }

            stats = new List<GearStat>();
            foreach(var stat in data.IGear.Stats)
            {
                stats.Add(stat.Value);
            }

            return true;
        }

        public void MovePlayerToTruckSpawn()
        {
            EventRelayer.Instance.OnRequestMovePlayer(_workTruckPlayerSpawn);
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
                _truckGear.Add(GearType.Mower, new RuntimeGearData(GearType.Mower, mowerData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.FuelEdger, out var edgerData))
            {
                _truckGear.Add(GearType.Edger, new RuntimeGearData(GearType.Edger, edgerData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.ManualPushStriper, out var striperData))
            {
                _truckGear.Add(GearType.Striper, new RuntimeGearData(GearType.Striper, striperData, null, null));
            }

            if (dataManager.GetGearData(GearVariant.FuelVacuum, out var vacuumData))
            {
                _truckGear.Add(GearType.Vacuum, new RuntimeGearData(GearType.Vacuum, vacuumData, null, null));
            }

            SpawnGear();
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
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 60, 250, 30), $"{_equippedGear.IGear?.EnergyStat}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 90, 250, 30), $"{_equippedGear.IGear?.DurabilityStat}", fontStyle);
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
