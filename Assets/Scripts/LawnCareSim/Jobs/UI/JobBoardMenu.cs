using LawnCareSim.Events;
using LawnCareSim.Input;
using LawnCareSim.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

namespace LawnCareSim.Jobs
{
    public class JobBoardMenu : BaseMenu
    {
        public static JobBoardMenu Instance;

        #region Constants
        private const float SELECTION_RATE = 0.01f;
        #endregion

        #region Private Vars
        private bool _refreshJobPosts;
        private List<PotentialJobUIComponent> _potentialJobsList;
        private PotentialJobUIComponent _hoveredEntry;
        private Vector2 _canvasCenter;
        private Vector2 _detailsModalSize;
        private float _selectProgress = 0f;
        #endregion

        #region Serialized Vars
        [SerializeField] private Transform _jobsList;
        [SerializeField] private RectTransform _jobDetailsModal;
        [SerializeField] private TextMeshProUGUI _jobNameText;
        [SerializeField] private TextMeshProUGUI _budgetText;
        [SerializeField] private Transform _difficultyStars;
        [SerializeField] private Transform _gearList;
        [SerializeField] private Transform _selectionGroup;
        [SerializeField] private Animator _selectProgressRadialAnimator;
        #endregion

        private void Awake()
        {
            Instance = this;
        }

        public override void InitializeMenuView()
        {
            base.InitializeMenuView();

            _refreshJobPosts = true;
            _detailsModalSize = _jobDetailsModal.sizeDelta;
            _canvasCenter = _canvas.renderingDisplaySize / 2;

            _jobDetailsModal.gameObject.SetActive(false);

            SetupObjectPool();

            EventRelayer.Instance.DayChangedEvent += DayChangedEventListener;
            InputController.Instance.MenuConfirmEvent += MenuConfirmEventListener;
            InputController.Instance.MenuConfirmCanceledEvent += MenuConfirmCanceledEventListener;
        }

        #region Event Listener
        private void DayChangedEventListener(object sender, Time.Day args)
        {
            _refreshJobPosts = true;
        }

        private void MenuConfirmEventListener(object sender, System.EventArgs e)
        {
            HandleSelectionUI(true);
        }

        private void MenuConfirmCanceledEventListener(object sender, System.EventArgs e)
        {
            HandleSelectionUI(false);
        }
        #endregion

        #region Menu Interface
        public override void Open()
        {
            if (_refreshJobPosts)
            {
                CreateJobPosts();
            }

            _selectionGroup.gameObject.SetActive(JobManager.Instance.ActiveJob == null);

            base.Open();
        }

        public override void Close()
        {
            base.Close();
            _jobDetailsModal.gameObject.SetActive(false);
        }
        #endregion

        #region UI Management
        #region Creation
        private void SetupObjectPool()
        {
            _potentialJobsList = new List<PotentialJobUIComponent>();
            foreach(Transform child in _jobsList)
            {
                var uiComp = child.GetComponent<PotentialJobUIComponent>();
                uiComp.AddEventListener(EventTriggerType.PointerEnter, (data) =>
                {
                    _hoveredEntry = uiComp;
                    ShowDetailsPanel();
                });
                uiComp.AddEventListener(EventTriggerType.PointerExit, (data) =>
                {
                    HideDetailsPanel();
                    _hoveredEntry = null;
                    _selectProgress = 0f;
                });

                _potentialJobsList.Add(uiComp);
            }
        }

        private void CreateJobPosts()
        {
            var dailyJobs = JobManager.Instance.DailyJobs;

            for(int i = 0; i < dailyJobs.Count; i++)
            {
                _potentialJobsList[i].BackingData = dailyJobs[i];
            }

            // Hide unused job items
            if (_potentialJobsList.Count > dailyJobs.Count)
            {
                for (int j = dailyJobs.Count; j < _potentialJobsList.Count; j++)
                {
                    _potentialJobsList[j].gameObject.SetActive(false);
                    _potentialJobsList[j].Clear();
                }
            }

            _refreshJobPosts = false;
        }
        #endregion

        #region Details
        private void ShowDetailsPanel()
        {
            if (_hoveredEntry.HasBeenRipped)
            {
                return;
            }

            Job hoveredJob = (Job)_hoveredEntry.BackingData;

            for (int i = 0; i < _difficultyStars.childCount; i++)
            {
                _difficultyStars.GetChild(i).gameObject.SetActive(i < hoveredJob.Difficulty);
            }

            for (int j = 0; j < _gearList.childCount; j++)
            {
                _gearList.GetChild(j).gameObject.SetActive(j < hoveredJob.Difficulty);
            }

            RectTransform hoveredRect = _hoveredEntry.GetComponent<RectTransform>();
            Vector3 newPosition = _hoveredEntry.transform.position;
            if (newPosition.x > _canvasCenter.x)
            {
                newPosition.x -= (_detailsModalSize.x / 2) + (hoveredRect.sizeDelta.x / 2) + 25;
            }
            else
            {
                newPosition.x += (_detailsModalSize.x / 2) + (hoveredRect.sizeDelta.x / 2) + 25;
            }

            if (newPosition.y < _canvasCenter.y - (hoveredRect.sizeDelta.y / 2))
            {
                newPosition.y += _detailsModalSize.y / 5;
            }
            else if(newPosition.y > _canvasCenter.y + (hoveredRect.sizeDelta.y / 2))
            {
                newPosition.y -= _detailsModalSize.y / 5;
            }

            _jobDetailsModal.position = newPosition;

            _jobDetailsModal.gameObject.SetActive(true);
        }

        private void HideDetailsPanel()
        {
            _jobDetailsModal.gameObject.SetActive(false);
        }

        private void HandleSelectionUI(bool start)
        {
            if (JobManager.Instance.ActiveJob != null)
            {
                return;
            }

            // Prevent messing with animator if its not visible
            if (!_selectionGroup.gameObject.activeInHierarchy)
            {
                return;
            }

            StopAllCoroutines();

            if (start)
            {
                StartCoroutine(ProgressSelectionRadial());
            }
            else
            {
                StartCoroutine(RegressSelectionRadial());
            }
        }

        private IEnumerator ProgressSelectionRadial()
        {
            while (_selectProgress < 1.0f)
            {
                if (_hoveredEntry == null)
                {
                    yield break;
                }

                _selectProgress = Mathf.Clamp(_selectProgress + SELECTION_RATE, 0f, 1.0f);
                _selectProgressRadialAnimator.Play("ProgressRadial", -1, _selectProgress);

                yield return new WaitForSeconds(0.01f);
            }

            OnJobSelected();
        }

        private IEnumerator RegressSelectionRadial()
        {
            while (_selectProgress > 0)
            {
                if (_hoveredEntry == null)
                {
                    yield break;
                }

                _selectProgress = Mathf.Clamp(_selectProgress - SELECTION_RATE, 0f, 1.0f);
                _selectProgressRadialAnimator.Play("ProgressRadial", -1, _selectProgress);

                yield return new WaitForSeconds(0.01f);
            }
        }
        #endregion
        #endregion

        private void OnJobSelected()
        {
            _selectionGroup.gameObject.SetActive(false);
            _hoveredEntry.OnSelected();
            JobManager.Instance.ActiveJob = (Job)_hoveredEntry.BackingData;
        }
    }
}
