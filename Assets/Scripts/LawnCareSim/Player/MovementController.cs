using Core.Utility;
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
        [SerializeField] private float _rotationSpeed = 3.0f;

        private Rigidbody _rigidbody;
        private GameObject _leftTurnPivot;
        private GameObject _rightTurnPivot;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _leftTurnPivot = GameObject.Find("LeftTurnAxis");
            _rightTurnPivot = GameObject.Find("RightTurnAxis");

            InputController.Instance.MoveEvent += MoveEventListener;
        }

        private void OnGUI()
        {
            var width = UnityEngine.Camera.main.pixelWidth;
            var height = UnityEngine.Camera.main.pixelHeight;
            Rect mainRect = new Rect(width * 0.2f, height * 0.04f, 300, 200);

            GUIStyle fontStyle = GUI.skin.label;
            fontStyle.fontSize = 20;

            GUI.Box(mainRect, GUIContent.none);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y, 250, 30), $"Horz {_horizontalInput} |Vert: {_verticalInput}", fontStyle);
            GUI.Label(new Rect(mainRect.x + 5, mainRect.y + 30, 250, 30), $"Velocity: {_rigidbody.velocity}", fontStyle);
        }

        private void Update()
        {
            
            ProcessMovementInput();
            //ProcessMowerMovement();
        }

        #region Event Listeners
        private void MoveEventListener(object sender, Vector2 e)
        {
            _horizontalInput = e.x;
            _verticalInput = e.y;
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

        // Move Speed: 10
        private void ProcessMovementInput()
        {
            if (!_canMove || _isBeingMovedManually)
            {
                return;
            }

            Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, 0.1f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                _rigidbody.velocity = _moveDirection * _movementSpeed;
            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        // Move Speed: 5
        // Rotation Speed: 75
        private void ProcessMowerMovement()
        {
            if (!_canMove || _isBeingMovedManually)
            {
                return;
            }

            Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

            // Only move if forward or backward input is given
            if (!MathHelpers.IsWithinRange(direction.z, 0f, 0.1f))
            {
                // Rotating Left
                if (direction.x > 0.1f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    transform.RotateAround(_rightTurnPivot.transform.position, direction.z * Vector3.up, _rotationSpeed * Time.deltaTime);
                }
                
                // Rotating Right
                else if (direction.x < -0.1f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    transform.RotateAround(_leftTurnPivot.transform.position, direction.z * -Vector3.up, _rotationSpeed * Time.deltaTime);
                }

                // Otherwise move in forward direction
                else
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * transform.forward;

                    _rigidbody.velocity = _moveDirection * _movementSpeed;
                }
            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        private void TogglePlayerControl(bool enable)
        {
            _canMove = enable;
        }
    }
}
