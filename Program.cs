#nullable enable
using System;
using System.Drawing;
using System.Threading.Tasks;
using OCR_MATH_ASSISTANT.Services;
using OCR_MATH_ASSISTANT.Models;

namespace OCR_MATH_ASSISTANT
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Bienvenido al Asistente de Matemáticas Avanzadas por Captura de Pantalla");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Este sistema reconoce expresiones matemáticas complejas usando LaTeX-OCR");
            Console.WriteLine("Integrales, derivadas, fracciones, raíces, trigonometría y más.");
            Console.WriteLine("");

            // Verificar si LaTeX-OCR está disponible
            Console.WriteLine("Verificando servicio LaTeX-OCR...");
            bool isLatexOcrAvailable = await LatexOcrService.IsServiceAvailableAsync();
            if (!isLatexOcrAvailable)
            {
                Console.WriteLine("  ADVERTENCIA: LaTeX-OCR no está disponible.");
                Console.WriteLine("   Para reconocimiento matemático avanzado, ejecuta:");
                Console.WriteLine("   docker pull lukasblecher/pix2tex:api");
                Console.WriteLine("   docker run --rm -p 8502:8502 lukasblecher/pix2tex:api");
                Console.WriteLine("");
                Console.WriteLine("Continuando con OCR básico (Tesseract)...");
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine(" LaTeX-OCR está disponible - Reconocimiento matemático avanzado activado");
                Console.WriteLine("");
            }

            Console.WriteLine("Seleccione el tipo de captura:");
            Console.WriteLine("1. Pantalla completa");
            Console.WriteLine("2. Monitor activo (donde está el cursor)");
            Console.WriteLine("3. Área personalizada");
            Console.Write("Opción: ");

            string? opcionInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(opcionInput))
            {
                Console.WriteLine("Opción no seleccionada.");
                return;
            }

            Bitmap? captura = null;

            switch (opcionInput)
            {
                case "1":
                    captura = ScreenCaptureService.CaptureScreen();
                    break;
                case "2":
                    captura = ScreenCaptureService.CaptureActiveScreen();
                    break;
                case "3":
                    Console.WriteLine("Selecciona el área matemática con el mouse...");
                    Console.WriteLine("  - Click izquierdo y arrastra para seleccionar");
                    Console.WriteLine("  - ESC para cancelar");
                    Console.WriteLine("  - Esperando selección...");

                    Rectangle? selectedArea = AreaSelector.SelectArea();

                    if (selectedArea.HasValue)
                    {
                        Console.WriteLine($"Área seleccionada: X={selectedArea.Value.X}, Y={selectedArea.Value.Y}, Ancho={selectedArea.Value.Width}, Alto={selectedArea.Value.Height}");
                        captura = AreaSelector.CaptureArea(selectedArea.Value);
                    }
                    else
                    {
                        Console.WriteLine("Selección cancelada.");
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    return;
            }

            if (captura != null)
            {
                try
                {
                    // Opcional: Guardar la captura original para depuración
                    string rutaCaptura = $"captura_original_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    captura.Save(rutaCaptura, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine($"Captura guardada para depuración: {rutaCaptura}");
                    // System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaCaptura) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar la captura original: {ex.Message}");
                }

                Console.WriteLine("\n--- Procesando la imagen ---");

                string textoReconocido = string.Empty;
                bool usóLatexOcr = false;

                // Intentar usar LaTeX-OCR primero si está disponible
                if (isLatexOcrAvailable)
                {
                    Console.WriteLine("Usando LaTeX-OCR para reconocimiento matemático avanzado...");
                    textoReconocido = await LatexOcrService.RecognizeMathExpressionAsync(captura);
                    usóLatexOcr = !string.IsNullOrWhiteSpace(textoReconocido);

                    if (usóLatexOcr)
                    {
                        Console.WriteLine($"Expresión LaTeX reconocida: \"{textoReconocido}\"");

                        // Detectar tipo de expresión
                        var tipoExpresion = AdvancedMathEvaluator.DetectExpressionType(textoReconocido);
                        Console.WriteLine($"Tipo de expresión detectado: {tipoExpresion}");
                    }
                }

                // Si LaTeX-OCR falló o no está disponible, usar Tesseract
                if (!usóLatexOcr)
                {
                    Console.WriteLine("Usando OCR básico (Tesseract)...");
                    textoReconocido = OcrService.RecognizeText(captura);
                    Console.WriteLine($"Texto reconocido por OCR: \"{textoReconocido}\"");
                }

                if (!string.IsNullOrWhiteSpace(textoReconocido))
                {
                    try
                    {
                        string expresionProcesada;
                        string resultado;

                        if (usóLatexOcr)
                        {
                            // Procesar expresión LaTeX
                            expresionProcesada = AdvancedMathEvaluator.ConvertLatexToMathExpression(textoReconocido);
                            Console.WriteLine($"Expresión convertida para evaluación: \"{expresionProcesada}\"");
                            resultado = AdvancedMathEvaluator.EvaluateMathExpression(expresionProcesada);
                        }
                        else
                        {
                            // Procesar expresión básica
                            expresionProcesada = ExpressionEvaluator.CleanExpression(textoReconocido);
                            Console.WriteLine($"Expresión limpia para evaluar: \"{expresionProcesada}\"");

                            // Intentar evaluar con el método básico primero
                            try
                            {
                                double resultadoNumerico = ExpressionEvaluator.EvaluateExpression(expresionProcesada);
                                resultado = resultadoNumerico.ToString();
                            }
                            catch
                            {
                                // Si falla, intentar con el evaluador avanzado
                                resultado = AdvancedMathEvaluator.EvaluateMathExpression(expresionProcesada);
                            }
                        }

                        Console.WriteLine($"\n El resultado de la operación es: {resultado}");

                        // Mostrar información adicional si es LaTeX-OCR
                        if (usóLatexOcr)
                        {
                            Console.WriteLine($" Motor utilizado: LaTeX-OCR + MathNet.Symbolics");
                        }
                        else
                        {
                            Console.WriteLine($" Motor utilizado: Tesseract + DataTable.Compute");
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"\n Error de formato en la expresión: {ex.Message}");
                        Console.WriteLine(" Asegúrate de que el OCR reconoció correctamente una operación matemática válida.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n No se pudo evaluar la expresión matemática: {ex.Message}");
                        Console.WriteLine(" Intenta capturar una imagen más clara o verifica la expresión.");
                    }
                }
                else
                {
                    Console.WriteLine(" El OCR no pudo reconocer ningún texto en la captura.");
                    Console.WriteLine(" Asegúrate de que la imagen contiene texto matemático claro.");
                }

                // Liberar recursos de la captura
                captura.Dispose();
            }
            else
            {
                Console.WriteLine("No se pudo realizar la captura de pantalla.");
            }
        }
    }
}