# Configuración de LaTeX-OCR para Reconocimiento Matemático Avanzado

## Requisitos Previos

1. **Docker Desktop** instalado y ejecutándose
2. **.NET 8.0 SDK** instalado
3. **Conexión a internet** para descargar la imagen Docker

## Pasos de Configuración

### 1. Descargar y Ejecutar LaTeX-OCR

```bash
# Descargar la imagen Docker
docker pull lukasblecher/pix2tex:api

# Ejecutar el servicio en el puerto 8502
docker run --rm -p 8502:8502 lukasblecher/pix2tex:api
```

### 2. Verificar que el Servicio está Funcionando

Abre tu navegador web y navega a:
- **API Health Check**: http://localhost:8502/
- **Demo Interactiva**: http://localhost:8501/ (si ejecutas la demo)

Deberías ver una respuesta JSON indicando que el servicio está activo.

### 3. Compilar y Ejecutar el Proyecto .NET

```bash
# Restaurar paquetes NuGet
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

## Flujo de Trabajo

1. **Inicio**: La aplicación verifica si LaTeX-OCR está disponible
2. **Captura**: Selecciona el tipo de captura de pantalla
3. **Reconocimiento**: 
   - Si LaTeX-OCR está disponible → Usa reconocimiento matemático avanzado
   - Si no → Usa Tesseract OCR básico
4. **Procesamiento**: Convierte LaTeX a expresiones matemáticas evaluables
5. **Evaluación**: Calcula el resultado usando MathNet.Symbolics

## Características Soportadas

### Expresiones Matemáticas Reconocibles

- **Integrales**: `\int_{a}^{b} f(x) dx`
- **Derivadas**: `\frac{d}{dx} f(x)`
- **Fracciones**: `\frac{a}{b}`
- **Raíces**: `\sqrt{x}`
- **Potencias**: `x^{2}`
- **Funciones Trigonométricas**: `\sin(x)`, `\cos(x)`, `\tan(x)`
- **Logaritmos**: `\ln(x)`, `\log(x)`
- **Símbolos Griegos**: `\pi`, `\alpha`, `\beta`, etc.

### Ejemplos de Uso

1. **Integral Definida**:
   - Captura: `\int_{0}^{1} x^{2} dx`
   - Resultado: `1/3`

2. **Derivada**:
   - Captura: `\frac{d}{dx} \sin(x)`
   - Resultado: `cos(x)`

3. **Fracción Compleja**:
   - Captura: `\frac{x^{2} + 1}{x + 2}`
   - Resultado: `(x^2 + 1)/(x + 2)`

## Solución de Problemas

### LaTeX-OCR no está disponible

**Síntoma**: Mensaje "⚠️ ADVERTENCIA: LaTeX-OCR no está disponible"

**Solución**:
1. Verifica que Docker está ejecutándose
2. Confirma que el contenedor está activo:
   ```bash
   docker ps
   ```
3. Verifica el puerto:
   ```bash
   curl http://localhost:8502/
   ```

### Error de Conexión

**Síntoma**: "Error de conexión con LaTeX-OCR"

**Solución**:
1. Reinicia el contenedor Docker
2. Verifica que el puerto 8502 no esté en uso
3. Revisa la configuración de firewall

### Reconocimiento Incorrecto

**Síntoma**: El OCR no reconoce correctamente la expresión

**Solución**:
1. Asegúrate de que la imagen sea clara y nítida
2. Evita fondos complejos
3. Usa imágenes de resolución moderada (no demasiado grandes)
4. Verifica que el texto matemático sea legible

## Alternativas

Si no puedes usar Docker, puedes instalar LaTeX-OCR directamente:

```bash
# Instalar Python 3.7+
pip install "pix2tex[api]"

# Ejecutar el servidor
python -m pix2tex.api.run
```

## Rendimiento

- **Velocidad**: 1-3 segundos por expresión
- **Precisión**: >95% para expresiones matemáticas claras
- **Recursos**: Bajo consumo de CPU y memoria

## Soporte

Para problemas con LaTeX-OCR:
- **GitHub**: https://github.com/lukas-blecher/LaTeX-OCR
- **Documentación**: https://pix2tex.readthedocs.io/

Para problemas con el proyecto .NET:
- Revisa los mensajes de error en la consola
- Verifica que todos los paquetes NuGet estén instalados
- Confirma la configuración del proyecto en `version2.csproj`
