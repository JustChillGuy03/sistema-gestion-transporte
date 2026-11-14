using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Dtos.Vehiculo;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehiculosController : TransporteControllerBase
    {
        private readonly IVehiculoService _servicio;

        public VehiculosController(IVehiculoService servicio)
        {
            _servicio = servicio;
        }

        // GET: api/vehiculos
        [HttpGet]
        public async Task<ActionResult<List<VehiculoModel>>> GetVehiculos()
        {
            return Responder(await _servicio.GetAllAsync());
        }

        // GET: api/vehiculos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehiculoModel>> GetVehiculo(int id)
        {
            return Responder(await _servicio.GetByIdAsync(id));
        }

        // GET: api/vehiculos/disponibles?desde=2027-02-01T07:00&hasta=2027-02-01T08:00
        [HttpGet("disponibles")]
        public async Task<ActionResult<List<VehiculoModel>>> GetDisponibles([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            return Responder(await _servicio.GetDisponiblesAsync(desde, hasta));
        }

        // POST: api/vehiculos
        [HttpPost]
        public async Task<ActionResult<VehiculoModel>> PostVehiculo(VehiculoDtoAdd dto)
        {
            var resultado = await _servicio.SaveAsync(dto);
            if (!resultado.Success)
                return Error(resultado);

            return CreatedAtAction(nameof(GetVehiculo), new { id = resultado.Data!.Id }, resultado.Data);
        }

        // PUT: api/vehiculos/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<VehiculoModel>> PutVehiculo(int id, VehiculoDtoUpdate dto)
        {
            return Responder(await _servicio.UpdateAsync(id, dto));
        }

        // DELETE: api/vehiculos/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            return ResponderSinContenido(await _servicio.RemoveAsync(id));
        }
    }
}
