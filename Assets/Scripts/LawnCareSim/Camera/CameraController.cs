using UnityEngine;
using Cinemachine;
using System.Collections;
using LawnCareSim.Events;
using LawnCareSim.Player;

namespace LawnCareSim.Camera
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;

        private void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = PlayerRef.Instance.transform;
        }

    }
}
