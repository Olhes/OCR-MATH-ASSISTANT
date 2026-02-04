#nullable enable
using System.Drawing;
using System.Windows.Forms; // Necesitas este using para Screen, Cursor, Rectangle, Graphics

namespace YourNamespace.Services // Asegúrate de que el namespace sea consistente
{
    public static class ScreenCaptureService // Una clase estática es apropiada si no tiene estado
    {
        /// <summary>
        /// Captura la pantalla principal.
        /// </summary>
        /// <returns>Un Bitmap de la pantalla principal o null si no se encuentra.</returns>
        public static Bitmap? CaptureScreen()
        {
            if (Screen.PrimaryScreen == null)
            {
                System.Console.WriteLine("Error: No se encontró una pantalla principal para capturar.");
                return null;
            }
            
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            return CaptureRegionInternal(bounds);
        }

        /// <summary>
        /// Captura la pantalla donde se encuentra el cursor.
        /// </summary>
        /// <returns>Un Bitmap de la pantalla activa.</returns>
        public static Bitmap CaptureActiveScreen()
        {
            Screen activeScreen = Screen.FromPoint(Cursor.Position);
            Rectangle bounds = activeScreen.Bounds;
            return CaptureRegionInternal(bounds);
        }

        /// <summary>
        /// Captura una región específica de la pantalla.
        /// </summary>
        /// <param name="region">La región a capturar.</param>
        /// <returns>Un Bitmap de la región especificada.</returns>
        public static Bitmap CaptureRegion(Rectangle region)
        {
            // Asegúrate de que la región es válida y no vacía
            if (region.Width <= 0 || region.Height <= 0)
            {
                System.Console.WriteLine("Advertencia: Región de captura inválida o vacía. Usando una región predeterminada (0,0,100,100).");
                region = new Rectangle(0, 0, 100, 100); 
            }
            return CaptureRegionInternal(region);
        }

        /// <summary>
        /// Método interno para realizar la captura real, reutilizable por los otros métodos.
        /// </summary>
        /// <param name="bounds">Las dimensiones del área a capturar.</param>
        /// <returns>El Bitmap resultante de la captura.</returns>
        private static Bitmap CaptureRegionInternal(Rectangle bounds)
        {
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            }
            return screenshot;
        }
    }
}