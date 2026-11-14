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
    public class ViajesController : TransporteControllerBase
    {
        private readonly IViajeRepository _repositorio;

        public ViajesController(IViajeRepository repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/viajes
        // GET: api/viajes?fecha=2026-12-01&estado=Programado
        [HttpGet]
        public async Task<ActionResult<List<ViajeModel>>> GetViajes([FromQuery] DateTime? fecha, [FromQuery] EstadoViaje? estado)
        {
            return await _repositorio.GetViajesAsync(fecha, estado);
        }

        // GET: api/viajes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ViajeModel>> GetViaje(int id)
        {
            var viaje = await _repositorio.GetViajeAsync(id);
            if (viaje == null)
                return NoEncontrado($"No existe el viaje {id}.");

            return viaje;
        }

        // POST: api/viajes  -> valida disponibilidad del vehículo y del conductor
        [HttpPost]
        public async Task<ActionResult<ViajeModel>> PostViaje(ViajeAddModel modelo)
        {
            var viaje = modelo.ToEntity();
            await _repositorio.AddAsync(viaje);

            var creado = await _repositorio.GetViajeAsync(viaje.Id);
            return CreatedAtAction(nameof(GetViaje), new { id = viaje.Id }, creado);
        }

        // PUT: api/viajes/5  -> reprogramar (solo viajes Programados)
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ViajeModel>> PutViaje(int id, ViajeUpdateModel modelo)
        {
            var viaje = await _repositorio.GetByIdAsync(id);
            if (viaje == null)
                return NoEncontrado($"No existe el viaje {id}.");

            viaje.ActualizarDesde(modelo);
            await _repositorio.UpdateAsync(viaje);

            return (await _repositorio.GetViajeAsync(id))!;
        }

        // PATCH: api/viajes/5/estado  { "estado": "EnCurso" }
        [HttpPatch("{id:int}/estado")]
        public async Task<ActionResult<ViajeModel>> CambiarEstado(int id, CambiarEstadoViajeModel modelo)
        {
            await _repositorio.CambiarEstadoAsync(id, modelo.Estado!.Value);
            return (await _repositorio.GetViajeAsync(id))!;
        }

        // DELETE: api/viajes/5  (borrado lógico; solo Programados o Cancelados)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteViaje(int id)
        {
            var viaje = await _repositorio.GetByIdAsync(id);
            if (viaje == null)
                return NoEncontrado($"No existe el viaje {id}.");

            await _repositorio.RemoveAsync(viaje);
            return NoContent();
        }

        // GET: api/viajes/5/pasajeros
        [HttpGet("{id:int}/pasajeros")]
        public async Task<ActionResult<List<PasajeroModel>>> GetPasajeros(int id)
        {
            return await _repositorio.GetPasajerosAsync(id);
        }

        // POST: api/viajes/5/pasajeros/3  -> inscribe al empleado 3 (valida el cupo)
        [HttpPost("{id:int}/pasajeros/{empleadoId:int}")]
        public async Task<ActionResult<ViajeModel>> InscribirPasajero(int id, int empleadoId)
        {
            await _repositorio.InscribirPasajeroAsync(id, empleadoId);
            return (await _repositorio.GetViajeAsync(id))!;
        }

        // DELETE: api/viajes/5/pasajeros/3
        [HttpDelete("{id:int}/pasajeros/{empleadoId:int}")]
        public async Task<IActionResult> QuitarPasajero(int id, int empleadoId)
        {
            await _repositorio.QuitarPasajeroAsync(id, empleadoId);
            return NoContent();
        }
    }
}
