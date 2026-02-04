#nullable enable
using System;
using System.Data; // Necesitas este using para DataTable.Compute
using System.Text; // Necesitas este using para StringBuilder

namespace YourNamespace.Services
{
    public static class ExpressionEvaluator // Clase estática para servicios sin estado
    {
        /// <summary>
        /// Limpia el texto reconocido por OCR para que sea una expresión matemática válida.
        /// Este es un paso crítico y puede requerir ajustes basados en la precisión del OCR.
        /// </summary>
        /// <param name="rawExpression">La cadena de texto cruda del OCR.</param>
        /// <returns>La expresión limpia.</returns>
        public static string CleanExpression(string rawExpression)
        {
            if (string.IsNullOrWhiteSpace(rawExpression))
            {
                return string.Empty;
            }

            // Quitar espacios, saltos de línea y otros caracteres no deseados
            string cleaned = rawExpression.Replace(" ", "")
                                          .Replace("\n", "")
                                          .Replace("\r", "");

            // Reemplazos comunes de OCR que pueden ser problemáticos para expresiones matemáticas
            cleaned = cleaned.Replace("x", "*")      // 'x' como multiplicación
                             .Replace("X", "*")      // 'X' como multiplicación
                             .Replace(",", ".")      // Comas como decimales (ej. 1,5 -> 1.5)
                             .Replace("—", "-")      // Guión largo por guión normal
                             .Replace("=", "")      // Si OCR detecta '='
                             .Replace("_", "-");    // Guión bajo por guión (a veces OCR lo confunde)
            
            // Más reemplazos si identificas patrones de error comunes
            // cleaned = cleaned.Replace("o", "0"); // Cuidado con esto, 'o' es una letra. Solo si es un problema recurrente.

            // Filtrar solo caracteres válidos para una expresión matemática básica
            // (números, operadores +, -, *, /, %, paréntesis)
            StringBuilder sb = new StringBuilder();
            foreach (char c in cleaned)
            {
                if (char.IsDigit(c) || c == '.' || c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '(' || c == ')')
                {
                    sb.Append(c);
                }
                // Si quieres soportar potencia '^', tendrías que añadirla aquí,
                // pero recuerda que DataTable.Compute no la soporta directamente.
            }
            return sb.ToString();
        }

        /// <summary>
        /// Evalúa una expresión matemática simple utilizando System.Data.DataTable.Compute().
        /// </summary>
        /// <param name="expression">La expresión matemática como cadena de texto.</param>
        /// <returns>El resultado de la expresión.</returns>
        /// <exception cref="ArgumentException">Se lanza si la expresión es nula, vacía o inválida.</exception>
        public static double EvaluateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("La expresión a evaluar no puede estar vacía.");
            }

            // DataTable.Compute es útil para expresiones aritméticas simples.
            // Para lógica más avanzada (funciones matemáticas, variables), se recomienda una librería de parsing de expresiones.
            try
            {
                object result = new DataTable().Compute(expression, null);
                return Convert.ToDouble(result);
            }
            catch (SyntaxErrorException ex)
            {
                throw new ArgumentException($"Error de sintaxis en la expresión: {expression}. Detalles: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error al evaluar la expresión: {expression}. Detalles: {ex.Message}", ex);
            }
        }
    }
}