using Core.UI;
using LawnCareSim.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Gear
{
    public partial class GearUIComponent : BaseMenuUIComponent
    {
        [SerializeField] private Image _gearImage;
        private GearInfo _backingData;

        public override object BackingData
        {
            get => _backingData;
            set
            {
                _backingData = (GearInfo)value;
                UpdateInterface();
            }
        }

        public override void UpdateInterface()
        {
            if (_backingData.Variant == GearVariant.Invalid || _backingData.GearType == GearType.None)
            {
                Clear();
                return;
            }

            _gearImage.sprite = _backingData.Sprite;
            _gearImage.enabled = true;
        }

        public override void Clear(bool clearBackingData = false)
        {
            base.Clear(clearBackingData);

            _gearImage.enabled = false;
            _gearImage.sprite = null;
        }

        
    }
}
