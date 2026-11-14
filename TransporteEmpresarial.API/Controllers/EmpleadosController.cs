using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Dtos.Empleado;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmpleadosController : TransporteControllerBase
    {
        private readonly IEmpleadoService _servicio;

        public EmpleadosController(IEmpleadoService servicio)
        {
            _servicio = servicio;
        }

        // GET: api/empleados            -> todos
        // GET: api/empleados?tipo=Conductor
        [HttpGet]
        public async Task<ActionResult<List<EmpleadoModel>>> GetEmpleados([FromQuery] TipoEmpleado? tipo)
        {
            var resultado = tipo.HasValue
                ? await _servicio.GetPorTipoAsync(tipo.Value)
                : await _servicio.GetAllAsync();

            return Responder(resultado);
        }

        // GET: api/empleados/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmpleadoModel>> GetEmpleado(int id)
        {
            return Responder(await _servicio.GetByIdAsync(id));
        }

        // GET: api/empleados/3/historial  -> traslados como pasajero y viajes como conductor
        [HttpGet("{id:int}/historial")]
        public async Task<ActionResult<List<TrasladoModel>>> GetHistorial(int id)
        {
            return Responder(await _servicio.GetHistorialAsync(id));
        }

        // POST: api/empleados
        [HttpPost]
        public async Task<ActionResult<EmpleadoModel>> PostEmpleado(EmpleadoDtoAdd dto)
        {
            var resultado = await _servicio.SaveAsync(dto);
            if (!resultado.Success)
                return Error(resultado);

            return CreatedAtAction(nameof(GetEmpleado), new { id = resultado.Data!.Id }, resultado.Data);
        }

        // PUT: api/empleados/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EmpleadoModel>> PutEmpleado(int id, EmpleadoDtoUpdate dto)
        {
            return Responder(await _servicio.UpdateAsync(id, dto));
        }

        // DELETE: api/empleados/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            return ResponderSinContenido(await _servicio.RemoveAsync(id));
        }
    }
}
