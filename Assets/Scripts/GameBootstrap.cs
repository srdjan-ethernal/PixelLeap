using System.Collections.Generic;
using UnityEngine;

namespace PixelLeap
{
    /// <summary>
    /// Builds the entire playable platformer level at runtime: camera, player,
    /// ground, floating platforms, coins, patrolling enemies, a kill zone and a
    /// goal flag. The scene only needs ONE GameObject carrying this component,
    /// so there is nothing fragile to wire up by hand. Replace this with real
    /// scenes / Tilemaps once you start authoring content in the editor.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Palette")]
        public Color skyColor = new Color(0.45f, 0.72f, 0.95f);
        public Color groundColor = new Color(0.32f, 0.55f, 0.30f);
        public Color platformColor = new Color(0.55f, 0.40f, 0.28f);
        public Color playerColor = new Color(0.95f, 0.85f, 0.25f);
        public Color coinColor = new Color(1.0f, 0.82f, 0.10f);
        public Color enemyColor = new Color(0.85f, 0.25f, 0.30f);
        public Color goalColor = new Color(0.20f, 0.85f, 0.45f);

        private int _groundLayer;
        private Transform _player;
        private int _coinCount;

        private void Start()
        {
            _groundLayer = LayerMask.NameToLayer("Ground");
            if (_groundLayer < 0) _groundLayer = 0; // fallback to Default if layer missing

            Physics2D.gravity = new Vector2(0f, -30f);

            var gm = new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("HUD").AddComponent<Hud>();

            BuildCamera();
            BuildPlayer(out Vector3 spawn);
            BuildLevel();
            BuildKillZone();

            gm.Register(_player, spawn, _coinCount);
            var follow = Camera.main.GetComponent<CameraFollow>();
            if (follow != null) follow.target = _player;
        }

        private void BuildCamera()
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = skyColor;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0f, 2f, -10f);
            camGo.AddComponent<CameraFollow>();
        }

        private void BuildPlayer(out Vector3 spawn)
        {
            spawn = new Vector3(-8f, 2f, 0f);
            var go = MakeSprite("Player", playerColor, spawn, new Vector2(0.9f, 0.9f), sortingOrder: 5);
            go.tag = "Player";

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.92f, 0.98f); // slightly inset to avoid snagging on seams

            var pc = go.AddComponent<PlayerController2D>();
            pc.groundMask = 1 << _groundLayer;
            _player = go.transform;
        }

        private void BuildLevel()
        {
            // Ground segments with a gap (the gap is a pit over the kill zone).
            MakePlatform(new Vector3(-4f, -2f, 0f), new Vector2(20f, 1.5f));
            MakePlatform(new Vector3(22f, -2f, 0f), new Vector2(18f, 1.5f));

            // Floating platforms (a gentle ascending path).
            MakePlatform(new Vector3(2f, 0.5f, 0f), new Vector2(3f, 0.6f));
            MakePlatform(new Vector3(6f, 2.0f, 0f), new Vector2(3f, 0.6f));
            MakePlatform(new Vector3(10f, 3.2f, 0f), new Vector2(2.5f, 0.6f));
            MakePlatform(new Vector3(14f, 1.5f, 0f), new Vector2(3f, 0.6f));
            MakePlatform(new Vector3(18f, 0.0f, 0f), new Vector2(3f, 0.6f));

            // Coins scattered along the route.
            foreach (var p in new[]
            {
                new Vector3(-6f, -0.6f, 0f), new Vector3(-3f, -0.6f, 0f),
                new Vector3(2f, 1.6f, 0f), new Vector3(6f, 3.1f, 0f),
                new Vector3(10f, 4.3f, 0f), new Vector3(14f, 2.6f, 0f),
                new Vector3(18f, 1.1f, 0f), new Vector3(24f, -0.6f, 0f),
                new Vector3(27f, -0.6f, 0f),
            })
                MakeCoin(p);

            // Enemies patrolling the ground stretches.
            MakeEnemy(center: -2f, range: 4f, y: -0.85f, speed: 3f);
            MakeEnemy(center: 24f, range: 4f, y: -0.85f, speed: 3.5f);

            // Goal flag near the end.
            MakeGoal(new Vector3(30f, -0.4f, 0f));
        }

        private void BuildKillZone()
        {
            var go = new GameObject("KillZone");
            go.transform.position = new Vector3(10f, -12f, 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(200f, 2f);
            go.AddComponent<KillZone>();
        }

        // ---- helpers ----------------------------------------------------------

        private GameObject MakeSprite(string name, Color color, Vector3 pos, Vector2 size, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.Solid(color);
            sr.sortingOrder = sortingOrder;
            return go;
        }

        private void MakePlatform(Vector3 pos, Vector2 size)
        {
            var go = MakeSprite("Platform", platformColor, pos, size, sortingOrder: 0);
            go.layer = _groundLayer;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one; // collider scales with transform
        }

        private void MakeCoin(Vector3 pos)
        {
            var go = MakeSprite("Coin", coinColor, pos, new Vector2(0.45f, 0.45f), sortingOrder: 3);
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            go.AddComponent<Coin>();
            _coinCount++;
        }

        private void MakeEnemy(float center, float range, float y, float speed)
        {
            var go = MakeSprite("Enemy", enemyColor, new Vector3(center, y, 0f), new Vector2(0.8f, 0.8f), sortingOrder: 4);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic; // moved by script, still triggers collisions
            var p = go.AddComponent<Patroller>();
            p.Configure(center, range, speed);
        }

        private void MakeGoal(Vector3 pos)
        {
            var go = MakeSprite("Goal", goalColor, pos, new Vector2(0.6f, 3f), sortingOrder: 2);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = Vector2.one;
            go.AddComponent<LevelGoal>();
        }
    }
}
