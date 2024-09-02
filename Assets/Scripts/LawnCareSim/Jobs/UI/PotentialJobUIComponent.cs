using LawnCareSim.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LawnCareSim.Jobs
{
    public class PotentialJobUIComponent : BaseMenuUIComponent
    {
        [SerializeField] private Image _jobImage;
        [SerializeField] private GameObject _rippedJobImage;
        [SerializeField] private Image _pushPinImage;
        [SerializeField] private TextMeshProUGUI _difficultyText;

        private Vector3 _defaultScale;
        private Vector3 _hoverScale = new Vector3(1.2f, 1.2f, 1f);

        private Job _backingData;

        private bool _hasBeenRipped;
        public bool HasBeenRipped => _hasBeenRipped;

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

        public override void UpdateInterface()
        {
            _difficultyText.text = _backingData.Difficulty.ToString();
            _difficultyText.enabled = true;

            _rippedJobImage.SetActive(false);
            _jobImage.gameObject.SetActive(true);

            _hasBeenRipped = false;
        }

        public override void Clear(bool clearBackingData = false)
        {
            base.Clear(clearBackingData);

            _difficultyText.enabled = false;
            _difficultyText.text = "";

            _rippedJobImage.SetActive(false);
            _jobImage.gameObject.SetActive(true);

            _hasBeenRipped = false;
        }

        public override void OnHover(bool start)
        {
            if (_hasBeenRipped)
            {
                return;
            }

            _jobImage.transform.localScale = start ? _hoverScale : _defaultScale;
            _pushPinImage.enabled = !start;
        }

        public override void OnSelected()
        {
            _rippedJobImage.SetActive(true);
            _jobImage.gameObject.SetActive(false);
            _difficultyText.enabled = false;

            _hasBeenRipped = true;
        }
    }
}
