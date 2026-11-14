using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Dtos.Viaje;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ViajesController : TransporteControllerBase
    {
        private readonly IViajeService _servicio;

        public ViajesController(IViajeService servicio)
        {
            _servicio = servicio;
        }

        // GET: api/viajes
        // GET: api/viajes?fecha=2026-12-01&estado=Programado
        [HttpGet]
        public async Task<ActionResult<List<ViajeModel>>> GetViajes([FromQuery] DateTime? fecha, [FromQuery] EstadoViaje? estado)
        {
            return Responder(await _servicio.GetViajesAsync(fecha, estado));
        }

        // GET: api/viajes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ViajeModel>> GetViaje(int id)
        {
            return Responder(await _servicio.GetByIdAsync(id));
        }

        // POST: api/viajes  -> valida disponibilidad del vehículo y del conductor
        [HttpPost]
        public async Task<ActionResult<ViajeModel>> PostViaje(ViajeDtoAdd dto)
        {
            var resultado = await _servicio.SaveAsync(dto);
            if (!resultado.Success)
                return Error(resultado);

            return CreatedAtAction(nameof(GetViaje), new { id = resultado.Data!.Id }, resultado.Data);
        }

        // PUT: api/viajes/5  -> reprogramar (solo viajes Programados)
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ViajeModel>> PutViaje(int id, ViajeDtoUpdate dto)
        {
            return Responder(await _servicio.UpdateAsync(id, dto));
        }

        // PATCH: api/viajes/5/estado  { "estado": "EnCurso" }
        [HttpPatch("{id:int}/estado")]
        public async Task<ActionResult<ViajeModel>> CambiarEstado(int id, ViajeDtoCambiarEstado dto)
        {
            return Responder(await _servicio.CambiarEstadoAsync(id, dto));
        }

        // DELETE: api/viajes/5  (borrado lógico; solo Programados o Cancelados)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteViaje(int id)
        {
            return ResponderSinContenido(await _servicio.RemoveAsync(id));
        }

        // GET: api/viajes/5/pasajeros
        [HttpGet("{id:int}/pasajeros")]
        public async Task<ActionResult<List<PasajeroModel>>> GetPasajeros(int id)
        {
            return Responder(await _servicio.GetPasajerosAsync(id));
        }

        // POST: api/viajes/5/pasajeros/3  -> inscribe al empleado 3 (valida el cupo)
        [HttpPost("{id:int}/pasajeros/{empleadoId:int}")]
        public async Task<ActionResult<ViajeModel>> InscribirPasajero(int id, int empleadoId)
        {
            return Responder(await _servicio.InscribirPasajeroAsync(id, empleadoId));
        }

        // DELETE: api/viajes/5/pasajeros/3
        [HttpDelete("{id:int}/pasajeros/{empleadoId:int}")]
        public async Task<IActionResult> QuitarPasajero(int id, int empleadoId)
        {
            return ResponderSinContenido(await _servicio.QuitarPasajeroAsync(id, empleadoId));
        }
    }
}
