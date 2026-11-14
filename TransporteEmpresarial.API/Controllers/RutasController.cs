using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Dtos.Ruta;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RutasController : TransporteControllerBase
    {
        private readonly IRutaService _servicio;

        public RutasController(IRutaService servicio)
        {
            _servicio = servicio;
        }

        // GET: api/rutas
        [HttpGet]
        public async Task<ActionResult<List<RutaModel>>> GetRutas()
        {
            return Responder(await _servicio.GetAllAsync());
        }

        // GET: api/rutas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RutaModel>> GetRuta(int id)
        {
            return Responder(await _servicio.GetByIdAsync(id));
        }

        // GET: api/rutas/1/viajes  -> qué unidad y qué chofer cubren la ruta en cada viaje
        [HttpGet("{id:int}/viajes")]
        public async Task<ActionResult<List<ViajeModel>>> GetViajesDeRuta(int id)
        {
            return Responder(await _servicio.GetViajesDeRutaAsync(id));
        }

        // POST: api/rutas
        [HttpPost]
        public async Task<ActionResult<RutaModel>> PostRuta(RutaDtoAdd dto)
        {
            var resultado = await _servicio.SaveAsync(dto);
            if (!resultado.Success)
                return Error(resultado);

            return CreatedAtAction(nameof(GetRuta), new { id = resultado.Data!.Id }, resultado.Data);
        }

        // PUT: api/rutas/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RutaModel>> PutRuta(int id, RutaDtoUpdate dto)
        {
            return Responder(await _servicio.UpdateAsync(id, dto));
        }

        // DELETE: api/rutas/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRuta(int id)
        {
            return ResponderSinContenido(await _servicio.RemoveAsync(id));
        }
    }
}
