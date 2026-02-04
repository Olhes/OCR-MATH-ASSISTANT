#nullable enable
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YourNamespace.Services
{
    public static class LatexOcrService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string LatexOcrApiUrl = "http://localhost:8502/predict";

        /// <summary>
        /// Realiza el reconocimiento de expresiones matemáticas usando LaTeX-OCR API.
        /// </summary>
        /// <param name="image">La imagen Bitmap a reconocer.</param>
        /// <returns>El código LaTeX reconocido o una cadena vacía si hay un error.</returns>
        public static async Task<string> RecognizeMathExpressionAsync(Bitmap image)
        {
            try
            {
                // Convertir Bitmap a base64
                string base64Image = ConvertBitmapToBase64(image);
                
                // Crear el payload para la API
                var payload = new { image = base64Image };
                string jsonPayload = JsonConvert.SerializeObject(payload);
                
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                
                // Enviar solicitud a la API de LaTeX-OCR
                var response = await httpClient.PostAsync(LatexOcrApiUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LatexOcrResponse>(responseContent);
                    return result?.Latex ?? string.Empty;
                }
                else
                {
                    Console.WriteLine($"Error en la API de LaTeX-OCR: {response.StatusCode}");
                    return string.Empty;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión con LaTeX-OCR: {ex.Message}");
                Console.WriteLine("Asegúrate de que LaTeX-OCR está ejecutándose en http://localhost:8502");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el reconocimiento LaTeX-OCR: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Convierte un Bitmap a una cadena base64.
        /// </summary>
        /// <param name="bitmap">La imagen a convertir.</param>
        /// <returns>La cadena base64 de la imagen.</returns>
        private static string ConvertBitmapToBase64(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Verifica si el servicio LaTeX-OCR está disponible.
        /// </summary>
        /// <returns>True si el servicio está disponible, false en caso contrario.</returns>
        public static async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(LatexOcrApiUrl.Replace("/predict", "/"));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Clase para deserializar la respuesta de la API de LaTeX-OCR.
    /// </summary>
    public class LatexOcrResponse
    {
        [JsonProperty("latex")]
        public string? Latex { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }
    }
}
