using UnityEngine;

namespace PixelLeap
{
    /// <summary>Smoothly follows a target with an offset and optional min-Y clamp.</summary>
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(2f, 1.5f, -10f);
        public float smoothTime = 0.15f;
        public float minY = -1f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            if (desired.y < minY) desired.y = minY;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }
}
