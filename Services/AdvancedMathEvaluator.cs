#nullable enable
using System;
using System.Text.RegularExpressions;
using MathNet.Symbolics;

namespace YourNamespace.Services
{
    public static class AdvancedMathEvaluator
    {
        /// <summary>
        /// Convierte expresiones LaTeX a formato evaluable por MathNet.Symbolics.
        /// </summary>
        /// <param name="latexExpression">La expresión LaTeX a convertir.</param>
        /// <returns>La expresión convertida o una cadena vacía si hay error.</returns>
        public static string ConvertLatexToMathExpression(string latexExpression)
        {
            if (string.IsNullOrWhiteSpace(latexExpression))
                return string.Empty;

            try
            {
                // Limpiar la expresión LaTeX
                string cleaned = latexExpression.Trim();
                
                // Eliminar delimitadores LaTeX comunes
                cleaned = Regex.Replace(cleaned, @"\\[|\\]|\\\(|\\\)", "");
                cleaned = Regex.Replace(cleaned, @"\$\$|\$", "");
                
                // Convertir comandos LaTeX comunes a formato matemático
                cleaned = ConvertLatexCommands(cleaned);
                
                // Limpiar espacios extra
                cleaned = Regex.Replace(cleaned, @"\s+", " ");
                
                return cleaned.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir LaTeX: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Convierte comandos LaTeX específicos a su equivalente matemático.
        /// </summary>
        /// <param name="expression">La expresión con comandos LaTeX.</param>
        /// <returns>La expresión convertida.</returns>
        private static string ConvertLatexCommands(string expression)
        {
            // Fracciones: \frac{a}{b} -> a/b
            expression = Regex.Replace(expression, @"\\frac\{([^}]+)\}\{([^}]+)\}", "($1)/($2)");
            
            // Raíces cuadradas: \sqrt{x} -> sqrt(x)
            expression = Regex.Replace(expression, @"\\sqrt\{([^}]+)\}", "sqrt($1)");
            
            // Potencias: x^{2} -> x^2
            expression = Regex.Replace(expression, @"\^\\\{([^}]+)\\\}", "^($1)");
            
            // Subíndices: x_{i} -> x_i
            expression = Regex.Replace(expression, @"_\\\{([^}]+)\\\}", "_$1");
            
            // Integrales: \int_{a}^{b} f(x) dx -> integral(f(x), x, a, b)
            expression = Regex.Replace(expression, @"\\int_([^{}]*)\^([^{}]*)\s*([^\\s]+)", "integral($3, x, $1, $2)");
            expression = Regex.Replace(expression, @"\\int\s+([^\\s]+)", "integral($1, x)");
            
            // Derivadas: \frac{d}{dx} f(x) -> derivative(f(x), x)
            expression = Regex.Replace(expression, @"\\frac\{d\}\{dx\}\s*([^\\s]+)", "derivative($1, x)");
            
            // Paréntesis LaTeX: \left( y \right) -> (y)
            expression = Regex.Replace(expression, @"\\left\(|\\right\)", "(");
            expression = Regex.Replace(expression, @"\\left\[|\\right\]", "[");
            
            // Símbolos griegos comunes
            expression = expression.Replace("\\pi", "pi");
            expression = expression.Replace("\\alpha", "alpha");
            expression = expression.Replace("\\beta", "beta");
            expression = expression.Replace("\\gamma", "gamma");
            expression = expression.Replace("\\delta", "delta");
            expression = expression.Replace("\\theta", "theta");
            expression = expression.Replace("\\lambda", "lambda");
            expression = expression.Replace("\\mu", "mu");
            expression = expression.Replace("\\sigma", "sigma");
            expression = expression.Replace("\\phi", "phi");
            expression = expression.Replace("\\omega", "omega");
            
            // Operadores matemáticos
            expression = expression.Replace("\\times", "*");
            expression = expression.Replace("\\cdot", "*");
            expression = expression.Replace("\\div", "/");
            expression = expression.Replace("\\pm", "+-");
            expression = expression.Replace("\\mp", "-+");
            
            // Funciones trigonométricas
            expression = Regex.Replace(expression, @"\\sin\{?([^}]*)\}?", "sin($1)");
            expression = Regex.Replace(expression, @"\\cos\{?([^}]*)\}?", "cos($1)");
            expression = Regex.Replace(expression, @"\\tan\{?([^}]*)\}?", "tan($1)");
            expression = Regex.Replace(expression, @"\\sec\{?([^}]*)\}?", "sec($1)");
            expression = Regex.Replace(expression, @"\\csc\{?([^}]*)\}?", "csc($1)");
            expression = Regex.Replace(expression, @"\\cot\{?([^}]*)\}?", "cot($1)");
            
            // Logaritmos y exponenciales
            expression = Regex.Replace(expression, @"\\ln\{?([^}]*)\}?", "log($1)");
            expression = Regex.Replace(expression, @"\\log\{?([^}]*)\}?", "log($1)");
            expression = Regex.Replace(expression, @"\\exp\{?([^}]*)\}?", "exp($1)");
            
            return expression;
        }

        /// <summary>
        /// Evalúa una expresión matemática usando MathNet.Symbolics.
        /// </summary>
        /// <param name="expression">La expresión matemática a evaluar.</param>
        /// <returns>El resultado como cadena de texto.</returns>
        public static string EvaluateMathExpression(string expression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return "Expresión vacía";

                // Intentar parsear y evaluar la expresión
                var parsedExpression = Infix.ParseOrUndefined(expression);
                
                if (parsedExpression.IsUndefined)
                {
                    return "No se pudo interpretar la expresión";
                }

                // Simplificar la expresión
                var simplified = Algebraic.Simplify(parsedExpression);
                
                // Intentar evaluar numéricamente si es posible
                try
                {
                    var evaluated = Evaluate.Evaluate(simplified);
                    return evaluated.ToString();
                }
                catch
                {
                    // Si no se puede evaluar numéricamente, devolver la forma simplificada
                    return simplified.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Error al evaluar: {ex.Message}";
            }
        }

        /// <summary>
        /// Detecta el tipo de expresión matemática.
        /// </summary>
        /// <param name="latexExpression">La expresión LaTeX.</param>
        /// <returns>El tipo de expresión detectado.</returns>
        public static MathExpressionType DetectExpressionType(string latexExpression)
        {
            if (string.IsNullOrWhiteSpace(latexExpression))
                return MathExpressionType.Unknown;

            if (latexExpression.Contains("\\int"))
                return MathExpressionType.Integral;
            
            if (latexExpression.Contains("\\frac{d}{dx}") || latexExpression.Contains("derivative"))
                return MathExpressionType.Derivative;
            
            if (latexExpression.Contains("\\frac"))
                return MathExpressionType.Fraction;
            
            if (latexExpression.Contains("\\sqrt"))
                return MathExpressionType.Root;
            
            if (Regex.IsMatch(latexExpression, @"sin|cos|tan|sec|csc|cot"))
                return MathExpressionType.Trigonometric;
            
            if (Regex.IsMatch(latexExpression, @"log|ln|exp"))
                return MathExpressionType.Logarithmic;
            
            return MathExpressionType.Algebraic;
        }
    }

    public enum MathExpressionType
    {
        Unknown,
        Algebraic,
        Integral,
        Derivative,
        Fraction,
        Root,
        Trigonometric,
        Logarithmic
    }
}
