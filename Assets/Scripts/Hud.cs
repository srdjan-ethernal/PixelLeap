using UnityEngine;

namespace PixelLeap
{
    /// <summary>
    /// Minimal on-screen UI drawn with IMGUI so no Canvas wiring is required.
    /// Shows score, coins, lives, controls, and win/lose banners.
    /// </summary>
    public class Hud : MonoBehaviour
    {
        private GUIStyle _label;
        private GUIStyle _big;
        private Texture2D _shade;

        private void EnsureStyles()
        {
            if (_label != null) return;

            _label = new GUIStyle { fontSize = 22, fontStyle = FontStyle.Bold };
            _label.normal.textColor = Color.white;

            _big = new GUIStyle { fontSize = 48, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _big.normal.textColor = Color.white;

            _shade = new Texture2D(1, 1);
            _shade.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
            _shade.Apply();
        }

        private void OnGUI()
        {
            EnsureStyles();
            var gm = GameManager.Instance;
            if (gm == null) return;

            GUI.Label(new Rect(16, 12, 400, 30), $"Score: {gm.Score}", _label);
            GUI.Label(new Rect(16, 42, 400, 30), $"Lives: {Mathf.Max(gm.Lives, 0)}", _label);
            GUI.Label(new Rect(16, Screen.height - 36, 800, 30),
                "Move: A/D or ←/→     Jump: Space / W / ↑     Stomp enemies from above", _label);

            if (gm.HasWon)
                DrawBanner("YOU WIN!  🏁", $"Final score: {gm.Score}    Press R to play again");
            else if (gm.IsGameOver)
                DrawBanner("GAME OVER", $"Score: {gm.Score}    Press R to try again");
        }

        private void DrawBanner(string title, string subtitle)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _shade);
            GUI.Label(new Rect(0, Screen.height / 2f - 60, Screen.width, 60), title, _big);
            var sub = new GUIStyle(_big) { fontSize = 24 };
            GUI.Label(new Rect(0, Screen.height / 2f + 10, Screen.width, 40), subtitle, sub);
        }
    }
}
