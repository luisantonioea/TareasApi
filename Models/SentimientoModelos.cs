using Microsoft.ML.Data;

namespace TareasApi.Models
{
    // 1. Clases para el Request/Response de la API REST
    public class SentimientoRequest
    {
        public string Comentario { get; set; } = string.Empty;
    }

    public class SentimientoResponse
    {
        public string Comentario { get; set; } = string.Empty;
        public string Sentimiento { get; set; } = string.Empty;
    }

    // 2. Clases para el entrenamiento y predicción de ML.NET
    public class ComentarioDatos
    {
        public string Texto { get; set; } = string.Empty;
        public bool Etiqueta { get; set; } // true = Positivo, false = Negativo
    }

    public class ComentarioPrediccion
    {
        [ColumnName("PredictedLabel")]
        public bool Prediccion { get; set; }
    }
}