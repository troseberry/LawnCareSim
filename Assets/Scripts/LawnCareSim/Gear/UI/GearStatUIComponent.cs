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
        [SerializeField] private Image _fillImage;

        [SerializeField] private Color _green;
        [SerializeField] private Color _yellow;
        [SerializeField] private Color _red;

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
            _statNameText.text = _backingData.Name.ToString();
            _statNameText.enabled = true;

            _fillImage.color = GetColorForStatValue();

            _statSlider.value = _backingData.Value;
            _statSlider.enabled = true;

        }

        public override void Clear(bool clearBackingData)
        {
            base.Clear(clearBackingData);

            _statNameText.enabled = false;
            _statNameText.text = "";

            _statSlider.enabled = false;
            _statSlider.value = 0;
        }

        private Color GetColorForStatValue()
        {
            if (_backingData.Value <= 0.33)
            {
                return _red;
            }
            else if (_backingData.Value >= 0.66)
            {
                return _green;
            }
            else
            {
                return _yellow;
            }
        }
    }
}
