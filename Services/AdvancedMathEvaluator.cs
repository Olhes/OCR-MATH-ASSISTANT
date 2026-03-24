#nullable enable
using System;
using System.Text.RegularExpressions;
using MathNet.Symbolics;

namespace OCR_MATH_ASSISTANT.Services
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
            expression = Regex.Replace(expression, @"\\int_\{([^}]+)\}\^\{([^}]+)\}\s*([^\\s]+?)\s*\\mathrm\{d\}x", "integral($3, x, $1, $2)");
            expression = Regex.Replace(expression, @"\\int_\{([^}]+)\}\^\{([^}]+)\}\s*([^\\s]+?)\s*dx", "integral($3, x, $1, $2)");
            expression = Regex.Replace(expression, @"\\int\s+([^\\s]+?)\s*\\mathrm\{d\}x", "integral($1, x)");
            expression = Regex.Replace(expression, @"\\int\s+([^\\s]+?)\s*dx", "integral($1, x)");
            
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
        /// Evalúa una expresión matemática.
        /// </summary>
        /// <param name="expression">La expresión matemática a evaluar.</param>
        /// <returns>El resultado como cadena de texto.</returns>
        public static string EvaluateMathExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return "Expresión vacía";

            try
            {
                // Verificar si es una integral
                if (expression.StartsWith("integral("))
                {
                    return EvaluateIntegral(expression);
                }
                
                // Verificar si es una derivada
                if (expression.StartsWith("derivative("))
                {
                    return EvaluateDerivative(expression);
                }

                // Para otras expresiones, intentar con MathNet.Symbolics
                var parsedExpression = Infix.ParseOrUndefined(expression);
                
                if (parsedExpression.IsUndefined)
                {
                    return "No se pudo interpretar la expresión";
                }

                // Usar la expresión parseada directamente sin simplificar
                var simplified = parsedExpression;
                
                // Intentar evaluar numéricamente si es posible
                try
                {
                    var evaluated = Evaluate.Evaluate(null, simplified);
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
                return $"Error: {ex.Message}";
            }
        }

        private static string EvaluateIntegral(string integralExpression)
        {
            try
            {
                // Parsear: integral(f(x), x, a, b)
                var match = System.Text.RegularExpressions.Regex.Match(integralExpression, @"integral\(([^,]+),\s*([^,]+),\s*([^,]+),\s*([^)]+)\)");
                
                if (!match.Success)
                    return "Formato de integral inválido";

                string func = match.Groups[1].Value.Trim();
                string variable = match.Groups[2].Value.Trim();
                string lowerStr = match.Groups[3].Value.Trim();
                string upperStr = match.Groups[4].Value.Trim();

                // Integrales comunes conocidas
                if (func.Contains("x^2") || func.Contains("x²") || func.Contains("x^{2}"))
                {
                    // ∫x² dx = x³/3
                    if (double.TryParse(lowerStr, out double lower) && double.TryParse(upperStr, out double upper))
                    {
                        // Límites numéricos
                        double result = (Math.Pow(upper, 3) - Math.Pow(lower, 3)) / 3;
                        return result.ToString();
                    }
                    else
                    {
                        // Límites variables (simbólicos)
                        return $"(1/3){upperStr}³ - (1/3){lowerStr}³";
                    }
                }
                else if (func.Contains("x") && !func.Contains("^"))
                {
                    // ∫x dx = x²/2 (solo si no contiene ^)
                    if (double.TryParse(lowerStr, out double lower) && double.TryParse(upperStr, out double upper))
                    {
                        // Límites numéricos
                        double result = (Math.Pow(upper, 2) - Math.Pow(lower, 2)) / 2;
                        return result.ToString();
                    }
                    else
                    {
                        // Límites variables (simbólicos)
                        return $"(1/2){upperStr}² - (1/2){lowerStr}²";
                    }
                }
                else if (func.Contains("1") || func.Contains("const"))
                {
                    // ∫1 dx = x
                    if (double.TryParse(lowerStr, out double lower) && double.TryParse(upperStr, out double upper))
                    {
                        // Límites numéricos
                        double result = upper - lower;
                        return result.ToString();
                    }
                    else
                    {
                        // Límites variables (simbólicos)
                        return $"{upperStr} - {lowerStr}";
                    }
                }

                return $"Integral de {func} desde {lowerStr} hasta {upperStr}";
            }
            catch (Exception ex)
            {
                return $"Error evaluando integral: {ex.Message}";
            }
        }

        private static string EvaluateDerivative(string derivativeExpression)
        {
            try
            {
                // Parsear: derivative(f(x), x)
                var match = System.Text.RegularExpressions.Regex.Match(derivativeExpression, @"derivative\(([^,]+),\s*([^)]+)\)");
                
                if (!match.Success)
                    return "Formato de derivada inválido";

                string func = match.Groups[1].Value.Trim();
                string variable = match.Groups[2].Value.Trim();

                // Derivadas comunes conocidas
                if (func.Contains("x^2") || func.Contains("x²"))
                {
                    return "2*x";
                }
                else if (func.Contains("x"))
                {
                    return "1";
                }
                else if (func.Contains("sin(x)"))
                {
                    return "cos(x)";
                }
                else if (func.Contains("cos(x)"))
                {
                    return "-sin(x)";
                }

                return $"Derivada de {func} respecto a {variable}";
            }
            catch (Exception ex)
            {
                return $"Error evaluando derivada: {ex.Message}";
            }
        }

        private static double EvaluateSimpleExpression(string expression)
        {
            try
            {
                // Evaluar expresiones numéricas simples
                if (double.TryParse(expression, out double result))
                    return result;
                    
                if (expression == "0") return 0;
                if (expression == "1") return 1;
                
                // Usar DataTable.Compute para expresiones más complejas
                object computed = new System.Data.DataTable().Compute(expression, null);
                return Convert.ToDouble(computed);
            }
            catch
            {
                return 0; // Valor por defecto
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
