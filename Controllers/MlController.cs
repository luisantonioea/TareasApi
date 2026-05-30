using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using TareasApi.Models;

namespace TareasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MlController : ControllerBase
    {
        [HttpPost("sentimiento")]
        public ActionResult<SentimientoResponse> AnalizarSentimiento([FromBody] SentimientoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comentario))
                return BadRequest("El comentario no puede estar vacío.");

            // 1. Inicializar el contexto de ML.NET
            var mlContext = new MLContext();

            // 2. Crear un dataset simple (Hardcoded para la evaluación)
            var datosEntrenamiento = new[]
            {
                new ComentarioDatos { Texto = "La tarea fue completada correctamente y el sistema funciona bien", Etiqueta = true },
                new ComentarioDatos { Texto = "Excelente trabajo, todo perfecto", Etiqueta = true },
                new ComentarioDatos { Texto = "Muy buena herramienta, me encanta", Etiqueta = true },
                new ComentarioDatos { Texto = "El sistema no permite registrar nuevas tareas", Etiqueta = false },
                new ComentarioDatos { Texto = "Tiene muchos errores y es demasiado lento", Etiqueta = false },
                new ComentarioDatos { Texto = "Pésima experiencia, no funciona para nada", Etiqueta = false }
            };

            // Cargar los datos en IDataView
            var dataView = mlContext.Data.LoadFromEnumerable(datosEntrenamiento);

            // 3. Crear el Pipeline de entrenamiento
            // Transformamos el texto en números (Features) y usamos regresión logística
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ComentarioDatos.Texto))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ComentarioDatos.Etiqueta), 
                    featureColumnName: "Features"));

            // 4. Entrenar el modelo
            var modelo = pipeline.Fit(dataView);

            // 5. Crear el motor de predicción y evaluar el comentario del usuario
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ComentarioDatos, ComentarioPrediccion>(modelo);
            var resultado = predictionEngine.Predict(new ComentarioDatos { Texto = request.Comentario });

            // 6. Formatear la respuesta
            var response = new SentimientoResponse
            {
                Comentario = request.Comentario,
                Sentimiento = resultado.Prediccion ? "Positivo" : "Negativo"
            };

            return Ok(response);
        }
    }
}