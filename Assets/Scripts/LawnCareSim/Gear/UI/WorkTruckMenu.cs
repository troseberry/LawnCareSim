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

        [SerializeField] private GameObject _detailsGroup;
        [SerializeField] private GameObject _gearGroup;
        [SerializeField] private TextMeshProUGUI _gearNameText;

        public static WorkTruckMenu Instance;

        private GearManager _gearManager;

        private List<GearInfo> _gearData = new List<GearInfo>();
        private List<(GameObject, GearUIComponent)> _gearList = new List<(GameObject, GearUIComponent)>();
        private (GameObject, GearUIComponent) _selectedEntry;

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
                    var gearGO = Instantiate(_gearComponentPrefab, _gearGroup.transform);

                    var gearComp = gearGO.GetComponent<GearUIComponent>();
                    gearComp.BackingData = _gearData[i];

                    var entry = (gearGO, gearComp);
                    _gearList.Add(entry);

                    EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
                    pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                    pointerEnterEntry.callback.AddListener((data) =>
                    {
                        _selectedEntry = entry;
                        ShowDetailsPanel();
                    });

                    EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
                    pointerExitEntry.eventID = EventTriggerType.PointerExit;
                    pointerExitEntry.callback.AddListener((data) =>
                    {
                        HideDetailsPanel();
                    });

                    var triggerComp = gearGO.GetComponent<EventTrigger>();
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

        private void ShowDetailsPanel()
        {
            _gearNameText.text = ((GearInfo)_selectedEntry.Item2.BackingData).Variant.ToString();

            Vector3 newPosition = _detailsGroup.transform.position;
            newPosition.x = _selectedEntry.Item1.transform.position.x;
            _detailsGroup.transform.position = newPosition;

            _detailsGroup.SetActive(true);
        }

        private void HideDetailsPanel()
        {
            _detailsGroup.SetActive(false);
        }
    }
}
