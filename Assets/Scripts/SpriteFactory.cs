using System.Collections.Generic;
using UnityEngine;

namespace PixelLeap
{
    /// <summary>
    /// Creates simple solid-color unit sprites at runtime so the game needs no
    /// imported art. A "unit sprite" is 1x1 world unit; scale GameObjects via
    /// transform.localScale to size them.
    /// </summary>
    public static class SpriteFactory
    {
        private static readonly Dictionary<Color32, Sprite> _cache = new Dictionary<Color32, Sprite>();

        public static Sprite Solid(Color color)
        {
            Color32 key = color;
            if (_cache.TryGetValue(key, out Sprite cached))
                return cached;

            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color32[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = key;
            tex.SetPixels32(pixels);
            tex.Apply();

            // pixelsPerUnit = 4 so the 4x4 texture maps to exactly 1 world unit.
            var sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
            sprite.name = "Solid_" + ColorUtility.ToHtmlStringRGB(color);
            _cache[key] = sprite;
            return sprite;
        }
    }
}
