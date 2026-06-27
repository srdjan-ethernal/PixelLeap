using UnityEngine;

namespace PixelLeap
{
    /// <summary>A trigger below the level that kills the player on contact (pits/falls).</summary>
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameManager.Instance != null) GameManager.Instance.PlayerDied();
        }
    }
}
