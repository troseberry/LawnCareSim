using LawnCareSim.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Jobs
{
    public class PotentialJobUIComponent : BaseMenuUIComponent
    {
        [SerializeField] private Image _jobImage;
        [SerializeField] private Image _pushPinImage;
        [SerializeField] private TextMeshProUGUI _difficultyText;

        private Vector3 _defaultScale;
        private Vector3 _hoverScale = new Vector3(1.2f, 1.2f, 1f);

        private Job _backingData;

        public override object BackingData
        {
            get => _backingData;
            set
            {
                _backingData = (Job)value;
                UpdateInterface();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _defaultScale = _jobImage.transform.localScale;
        }

        public override void OnHover(bool start)
        {
            _jobImage.transform.localScale = start ? _hoverScale : _defaultScale;
            _pushPinImage.enabled = !start;
        }
    }
}
