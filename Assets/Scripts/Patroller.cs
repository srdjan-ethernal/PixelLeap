using UnityEngine;

namespace PixelLeap
{
    /// <summary>
    /// An enemy that paces left/right between two bounds. The player kills it by
    /// stomping from above (and bounces); any other contact kills the player.
    /// </summary>
    public class Patroller : MonoBehaviour
    {
        public float speed = 2.5f;
        public float leftBound;
        public float rightBound;

        private int _dir = 1;
        private SpriteRenderer _sr;

        public void Configure(float center, float range, float moveSpeed)
        {
            leftBound = center - range;
            rightBound = center + range;
            speed = moveSpeed;
            transform.position = new Vector3(center, transform.position.y, transform.position.z);
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            transform.position += Vector3.right * _dir * speed * Time.deltaTime;
            if (transform.position.x >= rightBound && _dir > 0) _dir = -1;
            else if (transform.position.x <= leftBound && _dir < 0) _dir = 1;
            if (_sr != null) _sr.flipX = _dir < 0;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandlePlayer(collision.collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            HandlePlayer(collision.collider);
        }

        private void HandlePlayer(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var player = other.GetComponent<PlayerController2D>();
            var body = other.attachedRigidbody;

            // Stomp: the player's feet are above the enemy's center and the player
            // is moving downward. Position-based test avoids contact-normal sign
            // ambiguity between Unity versions.
            bool falling = body == null || body.velocity.y <= 0.5f;
            bool stomped = falling && other.bounds.min.y > transform.position.y;
            if (stomped)
            {
                if (player != null) player.Bounce();
                Destroy(gameObject);
            }
            else
            {
                if (GameManager.Instance != null) GameManager.Instance.PlayerDied();
            }
        }
    }
}
