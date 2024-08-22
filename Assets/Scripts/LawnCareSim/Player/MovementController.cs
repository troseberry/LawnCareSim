using Core.Utility;
using LawnCareSim.Events;
using LawnCareSim.Gear;
using LawnCareSim.Input;
using LawnCareSim.UI;
using System;
using UnityEngine;

namespace LawnCareSim.Player
{
    internal enum MovementMode
    {
        Default,
        Mower,
    }

    public partial class MovementController : MonoBehaviour
    {
        public static MovementController Instance;

        #region Variables
        [SerializeField] private GameObject _leftTurnAxis;
        [SerializeField] private GameObject _rightTurnAxis;

        private float _horizontalInput;
        private float _verticalInput;

        private bool _canMove = true;
        private bool _isBeingMovedManually = false;

        private MovementMode _currentMode = MovementMode.Default;
        private Vector3 _moveDirection;
        private Rigidbody _rigidbody;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _currentMoveSpeed = STANDARD_MOVE_SPEED;
            _rigidbody = GetComponent<Rigidbody>();

            InputController.Instance.MoveEvent += MoveEventListener;

            EventRelayer.Instance.GearSwitchedEvent += GearSwitchedEventListener;
            EventRelayer.Instance.MenuOpenedEvent += MenuOpenedEventListener;
            EventRelayer.Instance.MenuClosedEvent += MenuClosedEventListener;
            EventRelayer.Instance.MovePlayerEvent += MovePlayerEventListener;
            EventRelayer.Instance.DisablePlayerControlEvent += DisablePlayerControlEventListener;
        }


        private void Update()
        {
            if (!_canMove || _isBeingMovedManually)
            {
                return;
            }
                
            ProcessMovementInput();
        }
        #endregion

        #region Event Listeners
        private void MoveEventListener(object sender, Vector2 args)
        {
            _horizontalInput = args.x;
            _verticalInput = args.y;
        }

        private void DisablePlayerControlEventListener(object sender, bool args)
        {
            _canMove = !args;
        }

        private void MenuOpenedEventListener(object sender, MenuName menu)
        {
            _canMove = false;
        }

        private void MenuClosedEventListener(object sender, MenuName menu)
        {
            _canMove = true;
        }

        private void GearSwitchedEventListener(object sender, GearType args)
        {
            switch(args)
            {
                case GearType.None:
                case GearType.Edger:
                case GearType.Striper:
                case GearType.Vacuum:
                    _currentMode = MovementMode.Default;
                    UpdateMovementStats(args);
                    break;
                case GearType.Mower:
                    _currentMode = MovementMode.Mower;
                    break;
            }
        }

        private void MovePlayerEventListener(object sender, Transform args)
        {
            transform.position = args.position;
            transform.rotation = args.rotation;
        }
        #endregion
    }

    
    public partial class MovementController
    {
        private const float STANDARD_MOVE_SPEED = 10f;
        private const float PUSH_GEAR_MOVE_SPEED = 6.0f;
        private const float HELD_GEAR_MOVE_SPEED = 8.0f;
        private const float TURN_SMOOTH_TIME = 0.1f;
        private const float MOWER_MOVE_SPEED = 5.0f;
        private const float MOWER_ROTATION_SPEED = 75.0f;

        private float _currentTurnVelocity;
        private float _currentMoveSpeed;

        private void ProcessMovementInput()
        {
            switch (_currentMode)
            {
                case MovementMode.Default:
                    ExecuteStandardMovement();
                    break;
                case MovementMode.Mower:
                    ExecuteMowerMovement();
                    break;
            }
        }
        
        private void UpdateMovementStats(GearType newGear)
        {
            switch(newGear)
            {
                case GearType.Edger:
                case GearType.Vacuum:
                    _currentMoveSpeed = HELD_GEAR_MOVE_SPEED;
                    break;
                case GearType.Striper:
                    _currentMoveSpeed = PUSH_GEAR_MOVE_SPEED;
                    break;
                default:
                    _currentMoveSpeed = STANDARD_MOVE_SPEED;
                    break;
            }
        }

        private void ExecuteStandardMovement()
        {
            Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentTurnVelocity, TURN_SMOOTH_TIME);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                _rigidbody.velocity = _moveDirection * _currentMoveSpeed;
            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        private void ExecuteMowerMovement()
        {
            Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

            // Only move if forward or backward input is given
            if (!MathHelpers.IsWithinRange(direction.z, 0f, 0.1f))
            {
                // Rotating Left
                if (direction.x > 0.1f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    transform.RotateAround(_rightTurnAxis.transform.position, direction.z * Vector3.up, MOWER_ROTATION_SPEED * Time.deltaTime);
                }

                // Rotating Right
                else if (direction.x < -0.1f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    transform.RotateAround(_leftTurnAxis.transform.position, direction.z * -Vector3.up, MOWER_ROTATION_SPEED * Time.deltaTime);
                }

                // Otherwise move in forward direction
                else
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * transform.forward;

                    _rigidbody.velocity = _moveDirection * MOWER_MOVE_SPEED;
                }
            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }

    #region Debug
    public partial class MovementController
    {

        #region GUI
        /*
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
        */
        #endregion
    }
    #endregion
}
