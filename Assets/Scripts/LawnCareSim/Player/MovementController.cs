using LawnCareSim.Events;
using LawnCareSim.Input;
using LawnCareSim.UI;
using System.Collections;
using UnityEngine;

namespace LawnCareSim.Player
{
    public partial class MovementController : MonoBehaviour
    {
        public static MovementController Instance;

        [SerializeField] private float _movementSpeed = 10f;

        private CharacterController _characterController;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            InputController.Instance.MoveEvent += MoveEventListener;

            //EventRelayer.Instance.RequestMovePlayerEvent += MovePlayerRequestEventListener;
            //EventRelayer.Instance.CameraChangeStartedEvent += CameraChangeStartedEventListener;
            //EventRelayer.Instance.CameraChangeFinishedEvent += CameraChangeFinishedEventListener;

            //EventRelayer.Instance.MenuOpenedEvent += MenuOpenedEventListener;
            //EventRelayer.Instance.MenuClosedEvent += MenuClosedEventListener;

            //EventRelayer.Instance.PlayerStateChangedEvent += PlayerStateChangedEventListener;
        }

        private void Update()
        {
            ProcessMovementInput();
        }

        #region Event Listeners
        private void MoveEventListener(object sender, Vector2 e)
        {
            _horizontalInput = e.x;
            _verticalInput = e.y;
        }
        /*
        private void CameraChangeStartedEventListener(object sender, EventRelayer.CameraBlendData argse)
        {
            TogglePlayerControl(false);
        }

        private void CameraChangeFinishedEventListener(object sender, EventRelayer.CameraBlendData args)
        {
            TogglePlayerControl(true);
        }
        */

        private void MovePlayerRequestEventListener(object sender, Transform args)
        {
            _characterController.enabled = false;
            transform.position = args.position;
            _characterController.enabled = true;
        }

        private void MenuOpenedEventListener(object sender, MenuName menu)
        {
            TogglePlayerControl(false);
        }

        private void MenuClosedEventListener(object sender, MenuName menu)
        {
            TogglePlayerControl(true);
        }

        private void PlayerStateChangedEventListener(object sender, PlayerState state)
        {
            switch(state)
            {
                case PlayerState.Sleeping:
                    TogglePlayerControl(false);
                    break;
                default:
                    TogglePlayerControl(true);
                    break;
            }
        }
        #endregion
    }

    public partial class MovementController
    {
        private float _horizontalInput;
        private float _verticalInput;

        private bool _canMove = true;
        private bool _isBeingMovedManually = false;

        private Vector3 _moveDirection;
        private const float _turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;

        private MovementSpeed _currentSpeed = MovementSpeed.Idling;

        public MovementSpeed CurrentSpeed
        {
            get => _currentSpeed;
            private set
            {
                if (value != _currentSpeed)
                {
                    _currentSpeed = value;
                    EventRelayer.Instance.OnMovementSpeedChanged(_currentSpeed);
                }
            }
        }

        private void ProcessMovementInput()
        {
            if (!_canMove || _isBeingMovedManually)
            {
                return;
            }

            Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;// + _cameraController.transform.eulerAngles.y;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                _characterController.Move(_moveDirection * _movementSpeed * UnityEngine.Time.deltaTime);

                if (_currentSpeed != MovementSpeed.Walking)
                {
                    CurrentSpeed = MovementSpeed.Walking;
                }
            }
            else
            {
                _characterController.Move(new Vector3(0, -9.8f * UnityEngine.Time.deltaTime, 0));

                if (_currentSpeed != MovementSpeed.Idling)
                {
                    CurrentSpeed = MovementSpeed.Idling;
                }
            }
        }

        private void TogglePlayerControl(bool enable)
        {
            _canMove = enable;
            _characterController.enabled = enable;
        }

        public IEnumerator LerpToPoint(Transform destination)
        {
            _isBeingMovedManually = true;

            Vector3 startPos = transform.position;
            Vector3 startRot = transform.rotation.eulerAngles;

            float elapsedTime = 0f;
            float distance = Vector3.Distance(startPos, destination.position);
            while (distance > 0.1f)
            {
                transform.position = Vector3.Lerp(startPos, destination.position, elapsedTime / 0.5f);
                transform.eulerAngles = Vector3.Lerp(startRot, destination.eulerAngles, elapsedTime / 0.5f);

                distance = Vector3.Distance(transform.position, destination.position);
                elapsedTime += UnityEngine.Time.deltaTime;
                yield return null;
            }

            transform.position = destination.position;
            transform.eulerAngles = destination.eulerAngles;

            _isBeingMovedManually = false;

            yield return null;
        }

        
    }
}
