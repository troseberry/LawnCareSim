using UnityEngine;
using Cinemachine;
using System.Collections;
using LawnCareSim.Events;

namespace LawnCareSim.Camera
{
    public enum CameraName
    {
        Default,
        PlayerFollow
    }

    public class CameraController : MonoBehaviour
    {
        #region Fields
        public static CameraController Instance;

        private CinemachineBrain _cinemachineBrain;
        private float _defaultBlendTime;

        [SerializeField] private CinemachineVirtualCamera _currentCamera;
        #endregion

        #region Unity Events
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _cinemachineBrain = GetComponent<CinemachineBrain>();
            _defaultBlendTime = _cinemachineBrain.m_DefaultBlend.m_Time;
            _cinemachineBrain.m_CameraActivatedEvent.AddListener((blendToCamera, blendFromCamera) =>
            {
                StartCoroutine(CameraBlendStarted(blendToCamera, blendFromCamera));
            });
            
        }
        #endregion

        #region Event Listeners
        
        #endregion

        #region Camera Methods
        private IEnumerator CameraBlendStarted(ICinemachineCamera blendToCamera, ICinemachineCamera blendFromCamera)
        {
            var fromCam = GetCameraNameFromEntity(blendFromCamera);
            var toCam = GetCameraNameFromEntity(blendToCamera);

            EventRelayer.Instance.OnCameraChangeStarted(fromCam, toCam);

            yield return new WaitForSeconds(_defaultBlendTime);

            EventRelayer.Instance.OnCameraChangeFinished(fromCam, toCam);
        }

        private CameraName GetCameraNameFromEntity(ICinemachineCamera cineCam)
        {
            switch (cineCam.Name)
            {
                case "PlayerFollowCamera": return CameraName.PlayerFollow;
            }

            return CameraName.Default;
        }
        #endregion

    }
}
