using UnityEngine;

namespace PixelLeap
{
    /// <summary>Reaching this trigger wins the level.</summary>
    public class LevelGoal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameManager.Instance != null) GameManager.Instance.Win();
        }
    }
}
