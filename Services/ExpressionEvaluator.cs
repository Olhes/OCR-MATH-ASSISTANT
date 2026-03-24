#nullable enable
using System;
using System.Data; // Necesitas este using para DataTable.Compute
using System.Text; // Necesitas este using para StringBuilder

namespace OCR_MATH_ASSISTANT.Services
{
    public static class ExpressionEvaluator // Clase estática para servicios sin estado
    {
        /// <summary>
        /// Limpia el texto reconocido por OCR para que sea una expresión matemática válida.
        /// Mejorado para caracteres matemáticos comunes.
        /// </summary>
        /// <param name="rawExpression">La cadena de texto cruda del OCR.</param>
        /// <returns>La expresión limpia.</returns>
        public static string CleanExpression(string rawExpression)
        {
            if (string.IsNullOrWhiteSpace(rawExpression))
                return string.Empty;

            // Quitar espacios, saltos de línea y otros caracteres no deseados
            string cleaned = rawExpression.Replace(" ", "")
                                          .Replace("\n", "")
                                          .Replace("\r", "");

            // Reemplazos comunes de OCR para matemáticas
            cleaned = cleaned.Replace("×", "*")      // Símbolo de multiplicación
                             .Replace("÷", "/")      // Símbolo de división
                             .Replace("x", "*")      // 'x' como multiplicación
                             .Replace("X", "*")      // 'X' como multiplicación
                             .Replace("·", "*")      // Punto de multiplicación
                             .Replace(",", ".")      // Comas como decimales
                             .Replace("—", "-")      // Guión largo
                             .Replace("=", "")       // Eliminar iguales
                             .Replace("_", "-")      // Guión bajo
                             .Replace("²", "^2")     // Cuadrado
                             .Replace("³", "^3")     // Cubo
                             .Replace("√", "sqrt")   // Raíz cuadrada
                             .Replace("π", "3.14159") // Pi
                             .Replace("∞", "999999")  // Infinito (aproximado)
                             .Replace("∑", "sum")    // Sumatoria
                             .Replace("∫", "integral") // Integral
                             .Replace("∂", "derivative") // Derivada parcial
                             .Replace("∆", "delta")   // Delta
                             .Replace("α", "alpha")   // Alpha
                             .Replace("β", "beta")    // Beta
                             .Replace("γ", "gamma")   // Gamma
                             .Replace("θ", "theta")   // Theta
                             .Replace("λ", "lambda")  // Lambda
                             .Replace("μ", "mu")      // Mu
                             .Replace("σ", "sigma")   // Sigma
                             .Replace("φ", "phi")     // Phi
                             .Replace("ω", "omega")   // Omega
            ;

            // Filtrar caracteres válidos para expresión matemática
            StringBuilder sb = new StringBuilder();
            foreach (char c in cleaned)
            {
                if (char.IsDigit(c) || c == '.' || c == '+' || c == '-' || c == '*' || c == '/' || 
                    c == '%' || c == '(' || c == ')' || c == '^' || char.IsLetter(c))
                {
                    sb.Append(c);
                }
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