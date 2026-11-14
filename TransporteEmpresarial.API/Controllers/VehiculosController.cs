using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehiculosController : TransporteControllerBase
    {
        private readonly IVehiculoRepository _repositorio;

        public VehiculosController(IVehiculoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/vehiculos
        [HttpGet]
        public async Task<ActionResult<List<VehiculoModel>>> GetVehiculos()
        {
            var vehiculos = await _repositorio.GetAllAsync();
            return vehiculos.Select(v => v.ToModel()).ToList();
        }

        // GET: api/vehiculos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehiculoModel>> GetVehiculo(int id)
        {
            var vehiculo = await _repositorio.GetByIdAsync(id);
            if (vehiculo == null)
                return NoEncontrado($"No existe el vehículo {id}.");

            return vehiculo.ToModel();
        }

        // GET: api/vehiculos/disponibles?desde=2027-02-01T07:00&hasta=2027-02-01T08:00
        [HttpGet("disponibles")]
        public async Task<ActionResult<List<VehiculoModel>>> GetDisponibles([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            if (hasta <= desde)
                return DatosNoValidos("El parámetro 'hasta' debe ser posterior a 'desde'.");

            return await _repositorio.GetDisponiblesAsync(desde, hasta);
        }

        // POST: api/vehiculos
        [HttpPost]
        public async Task<ActionResult<VehiculoModel>> PostVehiculo(VehiculoAddModel modelo)
        {
            var vehiculo = modelo.ToEntity();
            await _repositorio.AddAsync(vehiculo);

            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.Id }, vehiculo.ToModel());
        }

        // PUT: api/vehiculos/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<VehiculoModel>> PutVehiculo(int id, VehiculoUpdateModel modelo)
        {
            var vehiculo = await _repositorio.GetByIdAsync(id);
            if (vehiculo == null)
                return NoEncontrado($"No existe el vehículo {id}.");

            vehiculo.ActualizarDesde(modelo);
            await _repositorio.UpdateAsync(vehiculo);

            return vehiculo.ToModel();
        }

        // DELETE: api/vehiculos/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _repositorio.GetByIdAsync(id);
            if (vehiculo == null)
                return NoEncontrado($"No existe el vehículo {id}.");

            await _repositorio.RemoveAsync(vehiculo);
            return NoContent();
        }
    }
}
