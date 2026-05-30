using System.ComponentModel.DataAnnotations;

namespace TareasApi.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public EstadoTarea Estado { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public PrioridadTarea Prioridad { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime FechaVencimiento { get; set; }
    }
}