#nullable enable
using System;
using System.Drawing;
using System.IO;
using Tesseract;
// using System.Drawing.Imaging; // You can keep this or remove it; the fully qualified name handles it.

namespace YourNamespace.Services
{
    public static class OcrService // Clase estática para servicios sin estado
    {
        private static readonly string TessDataPath = "./tessdata"; // Ruta a la carpeta tessdata

        /// <summary>
        /// Realiza el reconocimiento de texto en la imagen usando Tesseract OCR.
        /// </summary>
        /// <param name="image">La imagen Bitmap a reconocer.</param>
        /// <returns>El texto reconocido o una cadena vacía si hay un error.</returns>
        public static string RecognizeText(Bitmap image)
        {
            if (!System.IO.Directory.Exists(TessDataPath))
            {
                System.Console.WriteLine($"Error OCR: La carpeta 'tessdata' no se encontró en '{System.IO.Path.GetFullPath(TessDataPath)}'. " +
                                  "Asegúrate de descargar los archivos .traineddata y colocarlos allí.");
                return string.Empty;
            }

            try
            {
                using (var engine = new TesseractEngine(TessDataPath, "eng", EngineMode.Default))
                {
                    using (var ms = new MemoryStream())
                    {
                        // CORRECTED LINE: Specify the full namespace for ImageFormat
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);

                        using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                        {
                            using (var page = engine.Process(pix))
                            {
                                return page.GetText().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error durante el reconocimiento OCR: {ex.Message}");
                System.Console.WriteLine("Sugerencia: Asegúrate de que Tesseract está correctamente instalado y los 'tessdata' están en su lugar.");
                return string.Empty;
            }
        }
    }
}