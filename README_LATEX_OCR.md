# Configuración de LaTeX-OCR (Reconocimiento Matemático Avanzado)

## 🚀 Instalación Rápida

### Paso 1: Instalar LaTeX-OCR
Ejecuta el archivo `setup_latex_ocr.bat` para instalar LaTeX-OCR automáticamente.

### Paso 2: Iniciar el Servidor
Ejecuta `start_latex_ocr.bat` para iniciar el servidor LaTeX-OCR.

### Paso 3: Ejecutar tu Aplicación
```bash
dotnet run
```

## 📋 Requisitos
- Python 3.7 o superior
- Conexión a internet (para la instalación)

## 🎯 ¿Por qué LaTeX-OCR?

**Tesseract OCR vs LaTeX-OCR:**

| Característica | Tesseract | LaTeX-OCR |
|---------------|-----------|-----------|
| **Texto matemático** | ❌ Malo | ✅ Excelente |
| **Fracciones** | ❌ No reconoce | ✅ Perfecto |
| **Integrales** | ❌ No reconoce | ✅ Perfecto |
| **Derivadas** | ❌ No reconoce | ✅ Perfecto |
| **Símbolos griegos** | ❌ Confunde | ✅ Reconoce |
| **Notación LaTeX** | ❌ No soporta | ✅ Nativo |

## 🔧 Características de LaTeX-OCR

- **Precisión >95%** en expresiones matemáticas claras
- **Soporte completo** para notación LaTeX
- **Reconocimiento** de integrales, derivadas, fracciones, raíces
- **Símbolos griegos** y funciones especiales
- **Velocidad**: 1-3 segundos por expresión

## 📝 Ejemplos de Reconocimiento

### ✅ Lo que LaTeX-OCR reconoce bien:
```
∫₀¹ x² dx          → \int_{0}^{1} x^{2} dx
∂f/∂x             → \frac{\partial f}{\partial x}
Σᵢ₌₁ⁿ i²          → \sum_{i=1}^{n} i^{2}
√(x² + y²)        → \sqrt{x^{2} + y^{2}}
lim_{x→∞} (1/x)    → \lim_{x \to \infty} \frac{1}{x}
```

### ❌ Lo que Tesseract no reconoce:
```
∫₀¹ x² dx          → 0 1 x2 dx (incorrecto)
∂f/∂x             → af/ax (incorrecto)
√(x² + y²)        → - (x2 + y2) (incorrecto)
```

## 🚨 Solución de Problemas

### Si el servidor no inicia:
```bash
# Reinstalar pix2tex
pip uninstall pix2tex
pip install "pix2tex[api]"

# Iniciar manualmente
python -m pix2tex.api.run --port 8502
```

### Si hay errores de instalación:
```bash
# Actualizar pip
python -m pip install --upgrade pip

# Instalar dependencias manualmente
pip install torch torchvision timm transformers pillow numpy
pip install pix2tex
```

## 🌐 Verificación

Cuando el servidor esté funcionando, deberías ver:
```
Running on http://localhost:8502
```

Tu aplicación detectará automáticamente LaTeX-OCR y usará reconocimiento matemático avanzado.
