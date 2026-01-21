using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Settings")]
        public PlayerManager settings;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("GroundedCheck")]
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        // Trạng thái nội bộ
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private bool _isGrounded;

        // Animation ID để tối ưu hiệu năng (thay vì dùng string mỗi frame)
        private int _animIDMotionSpeed;
        private Animator animator;
		private bool _hasPlayedFallAnim = false;
        private string currAnim;

        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private const float _threshold = 0.01f;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";
#endif

        private void Awake()
        {
            if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // Gán ID cho tham số Float trong Animator (Giả sử bạn đặt tên tham số là "MotionSpeed")
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            animator = GetComponentInChildren<Animator>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            if (settings != null)
            {
                _jumpTimeoutDelta = settings.JumpTimeout;
                _fallTimeoutDelta = settings.FallTimeout;
            }
        }

        private void Update()
        {
            if (settings == null) return;

            GroundedCheck();
            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            if (settings == null) return;
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? settings.SprintSpeed : settings.MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // Tính toán tốc độ mượt mà
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > 0.1f)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * (_input.analogMovement ? _input.move.magnitude : 1f), Time.deltaTime * settings.SpeedChangeRate);
            }
            else _speed = targetSpeed;

            // --- CẬP NHẬT ANIMATION BLEND TREE TẠI ĐÂY ---
            UpdateMovementAnimation();

            // Thực hiện di chuyển
            Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void UpdateMovementAnimation()
        {
            if (animator == null) return;

            float animValue = 0f; // Mặc định là Idle (0)

            if (_isGrounded)
            {
                if (_input.move != Vector2.zero)
                {
                    // Nếu đang chạy (sprint) thì đặt là 1, nếu đi bộ thì là 0.5
                    animValue = _input.sprint ? 1.0f : 0.5f;
                }
            }

            // Sử dụng SetFloat để cập nhật Blend Tree
            animator.SetFloat(_animIDMotionSpeed, animValue, 0.1f, Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
				_hasPlayedFallAnim = false;
                _fallTimeoutDelta = settings.FallTimeout;
                if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(settings.JumpHeight * -2f * settings.Gravity);
                    // Có thể giữ Trigger cho Jump vì nhảy thường không dùng Blend Tree
                    if (animator != null) ChangeAnim("JumpUp");
                }

                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = settings.JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
                else if (_verticalVelocity < 0f && !_hasPlayedFallAnim)
                {
                    if (animator != null ) 
						ChangeAnim("JumpDown");
						_hasPlayedFallAnim = true;
                }
                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += settings.Gravity * Time.deltaTime;
            }
        }
        private void ChangeAnim(string anim)
        {
            if(anim != currAnim)
            {
                animator.SetTrigger(anim);
                currAnim = anim;
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float mult = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetPitch += _input.look.y * settings.RotationSpeed * mult;
                _rotationVelocity = _input.look.x * settings.RotationSpeed * mult;
                _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (settings == null) return;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}