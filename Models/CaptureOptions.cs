#nullable enable
using System.Drawing;

namespace YourNamespace.Models
{
    public class CaptureOptions
    {
        public CaptureType Type { get; set; }
        public Rectangle? Region { get; set; } // Nullable para la región
        // Podrías añadir más opciones aquí, como la calidad de la imagen, etc.
    }

    public enum CaptureType
    {
        FullScreen,
        ActiveScreen,
        CustomRegion
    }
}