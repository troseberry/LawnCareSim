
using Core.Utility;
using LawnCareSim.Events;
using TMPro;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class TaskHUD : MonoBehaviour
    {
        [SerializeField] private Animator _totalProgressRadialAnimator;
        private TextMeshProUGUI _totalProgressPercentageText;
        private float _progress = 0f;

        private void Start()
        {
            EventRelayer.Instance.ActiveJobProgressedEvent += ActiveJobProgressedEventListener;

            UIHelpers.SetUpUIElement(transform, ref _totalProgressPercentageText, "TotalProgressPercentageText");
            _totalProgressPercentageText.text = "0";
        }

        #region Event Listener
        private void ActiveJobProgressedEventListener(object sender, Job arg)
        {
            SetProgress(arg.TotalProgress);
        }
        #endregion

        private void SetProgress(float progress)
        {
            _totalProgressRadialAnimator.Play("TotalProgressRadial", -1, progress);
            _totalProgressPercentageText.text = $"{Mathf.FloorToInt(progress * 100)}";
        }
    }
}
