using System;
using DG.Tweening;
using UIModules.GameScreen.Scripts;
using UnityEngine;
using Zenject;

namespace GameScripts.PlayerScripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMoveModule : MonoBehaviour
    {
        [Inject] private Camera _camera;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float maxWalkSpeed = 10;
        [SerializeField] private float sensitivity = 5;
        private GameScreenUIView _gameScreenUIView;
        private CharacterController _characterController;
        private Joystick _joystick;
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private Vector2 _lookInput;
        private Vector3 _cameraStandPosition = new Vector3(0, 1.6f, 0f);
        private Vector3 _cameraCrouchPosition = new Vector3(0f,0.4f,0f);
        private const float PlayerHeight = 2f;
        private bool _isGrounded;
        private float _groundDistance = 0.4f;
        private float _gravity = -10f;
        private bool _isCrouching;
        private float _currentWalkSpeed;
        private float _cameraPitch;
        private int _rightFingerID;
        public event Action OnPlayerInsideShelter;
        
        public bool IsAlive { get; set; }

        public void Initialize(GameScreenUIView gameScreenUIView)
        {
            _camera = Camera.main;                      //TODO Камера берётся из попы!
            _gameScreenUIView = gameScreenUIView;
            _characterController = GetComponent<CharacterController>();
            _joystick = _gameScreenUIView.walkJoystick;
            _rightFingerID = -1;
            _gameScreenUIView.crouchButton.onClick.AddListener(CrouchGetUp);
            _currentWalkSpeed = maxWalkSpeed;
        }

        private void Update()
        {
            GetTouchInput();
        }

        private void FixedUpdate()
        {
            Walk();
            Gravity();
            if (_rightFingerID != -1)
                LookAround();
        }

        private void GetTouchInput()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        if (t.position.x > Screen.width / 2f && _rightFingerID == -1)
                            _rightFingerID = t.fingerId;
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (t.fingerId == _rightFingerID)
                        {
                            //Stop tracking the left finger
                            _rightFingerID = -1;
                        }

                        break;
                    case TouchPhase.Moved:
                        if (t.fingerId == _rightFingerID)
                        {
                            _lookInput = t.deltaPosition * (sensitivity * Time.deltaTime);
                        }

                        break;
                    case TouchPhase.Stationary:
                        if (t.fingerId == _rightFingerID)
                        {
                            _lookInput = Vector2.zero;
                        }

                        break;
                }
            }
        }

        private void LookAround()
        {
            _cameraPitch = Mathf.Clamp(_cameraPitch - _lookInput.y, -90, 90);
            _camera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
            transform.Rotate(transform.up, _lookInput.x);
        }

        private void Gravity()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;
            _velocity.y += _gravity * Time.deltaTime * Time.deltaTime;
            _characterController.Move(_velocity);
        }
        
        private void Walk()
        {
            _moveDirection = Vector3.zero;
            _moveDirection.x = _joystick.Horizontal;
            _moveDirection.z = _joystick.Vertical;
            var transform1 = transform;
            _moveDirection = transform1.right * _moveDirection.x + transform1.forward * _moveDirection.z +
                             transform1.up * _moveDirection.y;
            _characterController.Move(_moveDirection * (_currentWalkSpeed * Time.deltaTime));
        }

        private void CrouchGetUp()
        {
            if (_characterController.isGrounded && !_isCrouching)
            {
                _isCrouching = true;
                _characterController.height = 0.8f;
                _camera.transform.DOLocalMove(_cameraCrouchPosition, 0.25f);
                //_camera.transform.localPosition = _cameraCrouchPosition;
                _currentWalkSpeed = maxWalkSpeed / 2;
            }
            else if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up)))
            {
                _isCrouching = false;
                _characterController.height = 2f;
                _camera.transform.DOLocalMove(_cameraCrouchPosition, 0.25f);
                //_camera.transform.localPosition = _cameraCrouchPosition;
                _currentWalkSpeed = maxWalkSpeed;
            }
        }

        // private void GetUp()
        // {
        //     RaycastHit hit;
        //     bool wallUpHead = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up),
        //         out hit, PlayerHeight);
        //     if (_characterController.isGrounded && !wallUpHead)
        //     {
        //         _camera.transform.localPosition = new Vector3(0, PlayerHeight - 0.05f, 0);
        //         _currentWalkSpeed = maxWalkSpeed;
        //     }
        // }

        private void OnDisable()
        {
            _gameScreenUIView.crouchButton.onClick.RemoveListener(CrouchGetUp);
        }
    }
}