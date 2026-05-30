namespace TareasApi.Models
{
    // Clase para mapear exactamente lo que pide la evaluación
    public class ExternalTodoDto
    {
        public int ExternalId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool Completado { get; set; }
    }

    // Clase auxiliar para leer el JSON original de JsonPlaceholder
    public class JsonPlaceholderTodo
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}