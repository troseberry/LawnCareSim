using LawnCareSim.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

namespace LawnCareSim.Gear
{
    public class WorkTruckMenu : BaseMenu
    {

        [SerializeField] private GameObject _gearComponentPrefab;
        [SerializeField] private GameObject _gearStatPrefab;

        [SerializeField] private GameObject _detailsGroup;
        [SerializeField] private Transform _statsGroup;
        [SerializeField] private Transform _gearGroup;
        [SerializeField] private TextMeshProUGUI _gearNameText;

        private const int MAX_STATS_COUNT = 4;

        public static WorkTruckMenu Instance;

        private GearManager _gearManager;

        private List<GearInfo> _gearData = new List<GearInfo>();
        private List<(GameObject, GearUIComponent)> _gearList = new List<(GameObject, GearUIComponent)>();
        private (GameObject, GearUIComponent) _selectedEntry;

        private List<GameObject> _statObjects = new List<GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeMenuView();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                if (_state == Core.UI.MenuState.Closed)
                {
                    Open();
                }
                else if (_state == Core.UI.MenuState.Open)
                {
                    Close();
                }
            }
        }

        public override void InitializeMenuView()
        {
            base.InitializeMenuView();

            _gearManager = GearManager.Instance;

            _detailsGroup.SetActive(false);
        }

        public override void Open()
        {
            CreateGearSlots();
            CreateStatSlots();

            base.Open();
        }

        private void CreateGearSlots()
        {
            _gearData = _gearManager.GetAllGearInfo();

            for (int i = 0; i < _gearData.Count; i++)
            {
                // component gameobject already exists
                if (i < _gearList.Count)
                {
                    _gearList[i].Item2.BackingData = _gearData[i];
                }
                else
                {
                    var gearGO = Instantiate(_gearComponentPrefab, _gearGroup);

                    var gearComp = gearGO.GetComponent<GearUIComponent>();
                    gearComp.BackingData = _gearData[i];

                    var entry = (gearGO, gearComp);
                    _gearList.Add(entry);

                    Entry pointerClickEntry = new Entry { eventID = EventTriggerType.PointerClick };
                    pointerClickEntry.callback.AddListener((data) =>
                    {
                        InitiateGearSwitch();
                    });

                    Entry pointerEnterEntry = new Entry { eventID = EventTriggerType.PointerEnter };
                    pointerEnterEntry.callback.AddListener((data) =>
                    {
                        _selectedEntry = entry;
                        ShowDetailsPanel();
                    });

                    Entry pointerExitEntry = new Entry { eventID = EventTriggerType.PointerExit };
                    pointerExitEntry.callback.AddListener((data) =>
                    {
                        HideDetailsPanel();
                    });

                    var triggerComp = gearGO.GetComponent<EventTrigger>();
                    triggerComp.triggers.Add(pointerClickEntry);
                    triggerComp.triggers.Add(pointerEnterEntry);
                    triggerComp.triggers.Add(pointerExitEntry);
                }
            }

            //Hide unused gear slots
            if (_gearList.Count > _gearData.Count)
            {
                for (int j = _gearData.Count; j < _gearList.Count; j++)
                {
                    _gearList[j].Item1.SetActive(false);
                    _gearList[j].Item2.Clear();
                }
            }
        }

        private void CreateStatSlots()
        {
            if (_statObjects.Count < MAX_STATS_COUNT)
            {
                for (int i = _statObjects.Count; i < MAX_STATS_COUNT; i++)
                {
                    _statObjects.Add(Instantiate(_gearStatPrefab, _statsGroup));
                }
            }
        }

        private void FillStatSlots(GearType gearType)
        {
            if (!_gearManager.GetGearStatsForType(gearType, out var gearStats))
            {
                return;
            }
            
            if (gearStats.Count > MAX_STATS_COUNT)
            {
                Debug.LogError($"[{this}][{nameof(FillStatSlots)}] - Stats for gear {gearType} exceed UI maximum of {MAX_STATS_COUNT}");
                return;
            }

            for (int i = 0; i < gearStats.Count; i++)
            {
                _statObjects[i].SetActive(true);
                var comp = _statObjects[i].GetComponent<GearStatUIComponent>();
                comp.BackingData = gearStats[i];
            }

            if (gearStats.Count < _statObjects.Count)
            {
                for (int i = gearStats.Count; i < _statObjects.Count; i++)
                {
                    _statObjects[i].SetActive(false);
                }
            }

        }

        private void ShowDetailsPanel()
        {
            var currentInfo = ((GearInfo)_selectedEntry.Item2.BackingData);
            FillStatSlots(currentInfo.GearType);

            _gearNameText.text = currentInfo.Variant.ToString();

            Vector3 newPosition = _detailsGroup.transform.position;
            newPosition.x = _selectedEntry.Item1.transform.position.x;
            _detailsGroup.transform.position = newPosition;

            _detailsGroup.SetActive(true);
        }

        private void HideDetailsPanel()
        {
            _detailsGroup.SetActive(false);
        }

        private void InitiateGearSwitch()
        {
            var currentInfo = ((GearInfo)_selectedEntry.Item2.BackingData);
            _gearManager.SwitchGear(currentInfo.GearType, true);
        }
    }
}
