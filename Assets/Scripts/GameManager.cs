using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelLeap
{
    /// <summary>
    /// Holds run state (score, lives), handles death/respawn and win/lose.
    /// Created at runtime by GameBootstrap; reachable via GameManager.Instance.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int Score { get; private set; }
        public int Lives { get; private set; } = 3;
        public int TotalCoins { get; private set; }
        public bool HasWon { get; private set; }
        public bool IsGameOver { get; private set; }

        private Transform _player;
        private Rigidbody2D _playerBody;
        private Vector3 _spawnPoint;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Register(Transform player, Vector3 spawnPoint, int totalCoins)
        {
            _player = player;
            _playerBody = player.GetComponent<Rigidbody2D>();
            _spawnPoint = spawnPoint;
            TotalCoins = totalCoins;
        }

        public void AddScore(int amount)
        {
            Score += amount;
        }

        public void PlayerDied()
        {
            if (HasWon || IsGameOver) return;

            Lives--;
            if (Lives <= 0)
            {
                IsGameOver = true;
                return;
            }
            RespawnPlayer();
        }

        public void Win()
        {
            if (IsGameOver) return;
            HasWon = true;
        }

        private void RespawnPlayer()
        {
            if (_player == null) return;
            _player.position = _spawnPoint;
            if (_playerBody != null) _playerBody.velocity = Vector2.zero;
        }

        private void Update()
        {
            // Press R to restart after a win or game over.
            if ((HasWon || IsGameOver) && Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
