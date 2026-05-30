using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TareasApi.Models;

namespace TareasApi.Controllers
{
    [Route("api/tareas-externas")]
    [ApiController]
    public class TareasExternasController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TareasExternasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/tareas-externas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalTodoDto>>> GetTareasExternas()
        {
            var client = _httpClientFactory.CreateClient("JsonPlaceholder");

            try
            {
                var response = await client.GetAsync("todos");

                // Validación: Si la API externa no responde correctamente
                if (!response.IsSuccessStatusCode)
                    return StatusCode(503, "El servicio externo no está disponible en este momento.");

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var originalTodos = JsonSerializer.Deserialize<List<JsonPlaceholderTodo>>(content, options);

                if (originalTodos == null) return NotFound("No se encontraron tareas.");

                // Mapeo: No devolver el JSON original, pasarlo al DTO propio
                var result = originalTodos.Select(t => new ExternalTodoDto
                {
                    ExternalId = t.Id,
                    Titulo = t.Title,
                    Completado = t.Completed
                }).ToList();

                return Ok(result);
            }
            catch (Exception)
            {
                // Error controlado por si no hay internet o el servidor externo colapsa
                return StatusCode(503, "Ocurrió un error de conexión con la API externa.");
            }
        }

        // GET: api/tareas-externas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalTodoDto>> GetTareaExterna(int id)
        {
            var client = _httpClientFactory.CreateClient("JsonPlaceholder");

            try
            {
                var response = await client.GetAsync($"todos/{id}");

                // Validación: Si el ID no existe en la API externa
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound($"La tarea externa con ID {id} no fue encontrada.");

                // Validación: Si la API externa no responde
                if (!response.IsSuccessStatusCode)
                    return StatusCode(503, "El servicio externo no está disponible en este momento.");

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var originalTodo = JsonSerializer.Deserialize<JsonPlaceholderTodo>(content, options);

                if (originalTodo == null) return NotFound();

                // Mapeo al DTO propio
                var result = new ExternalTodoDto
                {
                    ExternalId = originalTodo.Id,
                    Titulo = originalTodo.Title,
                    Completado = originalTodo.Completed
                };

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(503, "Ocurrió un error de conexión con la API externa.");
            }
        }
    }
}