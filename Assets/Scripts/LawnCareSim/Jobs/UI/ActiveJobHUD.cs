
using LawnCareSim.Events;
using LawnCareSim.UI;
using TMPro;
using UnityEngine;

namespace LawnCareSim.Jobs
{
    public class ActiveJobHUD : BaseHUD
    {
        public static ActiveJobHUD Instance;

        #region Serialized Vars
        [SerializeField] private TextMeshProUGUI _budgetText;
        [SerializeField] private Transform _difficultyStars;
        [SerializeField] private Transform _gearList;
        #endregion

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            EventRelayer.Instance.ActiveJobSelectedEvent += ActiveJobSelectedEventListener;
        }

        private void ActiveJobSelectedEventListener(object sender, Job job)
        {
            //_budgetText.Text = "";

            for (int i = 0; i < _difficultyStars.childCount; i++)
            {
                _difficultyStars.GetChild(i).gameObject.SetActive(i < job.Difficulty);
            }

            for (int j = 0; j < _gearList.childCount; j++)
            {
                _gearList.GetChild(j).gameObject.SetActive(j < job.Difficulty);
            }
        }
    }
}
