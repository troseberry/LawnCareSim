using LawnCareSim.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Gear
{
    public class GearStatUIComponent : BaseMenuUIComponent
    {
        [SerializeField] private TextMeshProUGUI _statNameText;
        [SerializeField] private Slider _statSlider;

        private GearStat _backingData;

        public override object BackingData
        {
            get => _backingData;
            set
            {
                _backingData = (GearStat)value;
                UpdateInterface();
            }
        }

        public override void UpdateInterface()
        {
            _statNameText.text = _backingData.Name;
            _statNameText.enabled = true;

            _statSlider.value = _backingData.Value;
            _statSlider.enabled = false;
        }

        public override void Clear(bool clearBackingData)
        {
            base.Clear(clearBackingData);

            _statNameText.enabled = false;
            _statNameText.text = "";

            _statSlider.enabled = false;
            _statSlider.value = 0;
        }
    }
}
