using UnityEngine;

namespace PixelLeap
{
    /// <summary>
    /// Responsive 2D platformer movement: run, jump with coyote time + jump
    /// buffering, variable jump height, and sprite flip. Uses the legacy Input
    /// manager (arrow keys / A-D to move, Space / W / Up to jump).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 7f;
        public float acceleration = 60f;
        public float deceleration = 70f;

        [Header("Jump")]
        public float jumpForce = 14f;
        public float coyoteTime = 0.1f;
        public float jumpBuffer = 0.1f;
        public float fallMultiplier = 3.0f;
        public float lowJumpMultiplier = 2.2f;

        [Header("Ground Check")]
        public LayerMask groundMask;
        public float groundCheckRadius = 0.18f;
        public float bounceForce = 16f;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Transform _feet;
        private float _coyoteCounter;
        private float _bufferCounter;
        private bool _isGrounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            // World gravity is set to -30 by GameBootstrap; scale 1 => apex height
            // ~= jumpForce^2 / (2*30) ~= 3.3 units, enough to climb the platforms.
            _rb.gravityScale = 1f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            _feet = new GameObject("Feet").transform;
            _feet.SetParent(transform, false);
            _feet.localPosition = new Vector3(0f, -0.5f, 0f);
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");

            _isGrounded = Physics2D.OverlapCircle(_feet.position, groundCheckRadius, groundMask);
            _coyoteCounter = _isGrounded ? coyoteTime : _coyoteCounter - Time.deltaTime;

            bool jumpPressed = Input.GetKeyDown(KeyCode.Space) ||
                               Input.GetKeyDown(KeyCode.W) ||
                               Input.GetKeyDown(KeyCode.UpArrow);
            _bufferCounter = jumpPressed ? jumpBuffer : _bufferCounter - Time.deltaTime;

            if (_bufferCounter > 0f && _coyoteCounter > 0f)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                _bufferCounter = 0f;
                _coyoteCounter = 0f;
            }

            // Variable jump height: cut the jump short if the button is released.
            bool jumpHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            if (_rb.velocity.y > 0f && !jumpHeld)
                _rb.velocity += Vector2.up * Physics2D.gravity.y * _rb.gravityScale * (lowJumpMultiplier - 1f) * Time.deltaTime;
            else if (_rb.velocity.y < 0f)
                _rb.velocity += Vector2.up * Physics2D.gravity.y * _rb.gravityScale * (fallMultiplier - 1f) * Time.deltaTime;

            if (h > 0.01f) _sr.flipX = false;
            else if (h < -0.01f) _sr.flipX = true;
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float targetSpeed = h * moveSpeed;
            float speedDiff = targetSpeed - _rb.velocity.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            float movement = speedDiff * accelRate * Time.fixedDeltaTime;
            _rb.velocity = new Vector2(_rb.velocity.x + movement, _rb.velocity.y);
        }

        /// <summary>Called when the player stomps an enemy.</summary>
        public void Bounce()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, bounceForce);
        }

        private void OnDrawGizmosSelected()
        {
            if (_feet == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_feet.position, groundCheckRadius);
        }
    }
}
