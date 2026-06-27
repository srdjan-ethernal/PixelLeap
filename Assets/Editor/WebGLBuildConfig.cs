using UnityEditor;

namespace PixelLeap.EditorTools
{
    /// <summary>
    /// GitHub Pages serves files statically and cannot set the Content-Encoding
    /// headers that Unity's gzip/brotli WebGL builds require, which makes the
    /// player fail to load. Disabling compression makes the build work on Pages.
    /// Runs automatically whenever the editor loads (including CI batch mode).
    /// </summary>
    [InitializeOnLoad]
    public static class WebGLBuildConfig
    {
        static WebGLBuildConfig()
        {
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
        }
    }
}
