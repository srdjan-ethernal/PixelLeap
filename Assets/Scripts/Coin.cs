using UnityEngine;

namespace PixelLeap
{
    /// <summary>A pickup that bobs, spins, and adds to the score on touch.</summary>
    public class Coin : MonoBehaviour
    {
        public int value = 10;
        public float bobAmplitude = 0.15f;
        public float bobSpeed = 3f;
        public float spinSpeed = 180f;

        private Vector3 _origin;
        private float _phase;

        private void Start()
        {
            _origin = transform.position;
            _phase = transform.position.x; // de-sync coins so they don't bob in unison
        }

        private void Update()
        {
            float y = Mathf.Sin((Time.time + _phase) * bobSpeed) * bobAmplitude;
            transform.position = _origin + new Vector3(0f, y, 0f);
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameManager.Instance != null) GameManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}
