using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasApi.Data;
using TareasApi.Models;

namespace TareasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tareas (¡AQUÍ ESTÁ EL CAMBIO DE LA PREGUNTA 2!)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas(
            [FromQuery] EstadoTarea? estado,
            [FromQuery] PrioridadTarea? prioridad,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            // 1. Validación: fechaInicio no puede ser mayor que fechaFin
            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio > fechaFin)
            {
                return BadRequest("La fechaInicio no puede ser mayor que la fechaFin.");
            }

            // 2. Iniciar la consulta base
            var query = _context.Tareas.AsQueryable();

            // 3. Aplicar filtros dinámicamente si el usuario los envió
            if (estado.HasValue)
            {
                query = query.Where(t => t.Estado == estado.Value);
            }

            if (prioridad.HasValue)
            {
                query = query.Where(t => t.Prioridad == prioridad.Value);
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.Date >= fechaInicio.Value.Date);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.Date <= fechaFin.Value.Date);
            }

            // 4. Ejecutar la consulta y devolver los resultados
            return await query.ToListAsync();
        }

        // GET: api/tareas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();
            return tarea;
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea(Tarea tarea)
        {
            if (tarea.FechaVencimiento.Date < DateTime.UtcNow.Date)
                return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        // PUT: api/tareas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea tarea)
        {
            if (id != tarea.Id) return BadRequest();

            if (tarea.FechaVencimiento.Date < DateTime.UtcNow.Date)
                return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");

            _context.Entry(tarea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TareaExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/tareas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }
}