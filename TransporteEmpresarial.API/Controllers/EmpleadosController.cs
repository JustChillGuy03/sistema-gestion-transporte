using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmpleadosController : TransporteControllerBase
    {
        private readonly IEmpleadoRepository _repositorio;

        public EmpleadosController(IEmpleadoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/empleados            -> todos
        // GET: api/empleados?tipo=Conductor
        [HttpGet]
        public async Task<ActionResult<List<EmpleadoModel>>> GetEmpleados([FromQuery] TipoEmpleado? tipo)
        {
            if (tipo.HasValue)
                return await _repositorio.GetPorTipoAsync(tipo.Value);

            var empleados = await _repositorio.GetAllAsync();
            return empleados.Select(e => e.ToModel()).ToList();
        }

        // GET: api/empleados/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmpleadoModel>> GetEmpleado(int id)
        {
            var empleado = await _repositorio.GetByIdAsync(id);
            if (empleado == null)
                return NoEncontrado($"No existe el empleado {id}.");

            return empleado.ToModel();
        }

        // GET: api/empleados/3/historial  -> traslados como pasajero y viajes como conductor
        [HttpGet("{id:int}/historial")]
        public async Task<ActionResult<List<TrasladoModel>>> GetHistorial(int id)
        {
            return await _repositorio.GetHistorialAsync(id);
        }

        // POST: api/empleados
        [HttpPost]
        public async Task<ActionResult<EmpleadoModel>> PostEmpleado(EmpleadoAddModel modelo)
        {
            var empleado = modelo.ToEntity();
            await _repositorio.AddAsync(empleado);

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado.ToModel());
        }

        // PUT: api/empleados/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EmpleadoModel>> PutEmpleado(int id, EmpleadoUpdateModel modelo)
        {
            var empleado = await _repositorio.GetByIdAsync(id);
            if (empleado == null)
                return NoEncontrado($"No existe el empleado {id}.");

            empleado.ActualizarDesde(modelo);
            await _repositorio.UpdateAsync(empleado);

            return empleado.ToModel();
        }

        // DELETE: api/empleados/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _repositorio.GetByIdAsync(id);
            if (empleado == null)
                return NoEncontrado($"No existe el empleado {id}.");

            await _repositorio.RemoveAsync(empleado);
            return NoContent();
        }
    }
}
