using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RutasController : TransporteControllerBase
    {
        private readonly IRutaRepository _repositorio;

        public RutasController(IRutaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/rutas
        [HttpGet]
        public async Task<ActionResult<List<RutaModel>>> GetRutas()
        {
            var rutas = await _repositorio.GetAllAsync();
            return rutas.Select(r => r.ToModel()).ToList();
        }

        // GET: api/rutas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RutaModel>> GetRuta(int id)
        {
            var ruta = await _repositorio.GetByIdAsync(id);
            if (ruta == null)
                return NoEncontrado($"No existe la ruta {id}.");

            return ruta.ToModel();
        }

        // GET: api/rutas/1/viajes  -> qué unidad y qué chofer cubren la ruta en cada viaje
        [HttpGet("{id:int}/viajes")]
        public async Task<ActionResult<List<ViajeModel>>> GetViajesDeRuta(int id)
        {
            return await _repositorio.GetViajesDeRutaAsync(id);
        }

        // POST: api/rutas
        [HttpPost]
        public async Task<ActionResult<RutaModel>> PostRuta(RutaAddModel modelo)
        {
            var ruta = modelo.ToEntity();
            await _repositorio.AddAsync(ruta);

            return CreatedAtAction(nameof(GetRuta), new { id = ruta.Id }, ruta.ToModel());
        }

        // PUT: api/rutas/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RutaModel>> PutRuta(int id, RutaUpdateModel modelo)
        {
            var ruta = await _repositorio.GetByIdAsync(id);
            if (ruta == null)
                return NoEncontrado($"No existe la ruta {id}.");

            ruta.ActualizarDesde(modelo);
            await _repositorio.UpdateAsync(ruta);

            return ruta.ToModel();
        }

        // DELETE: api/rutas/5  (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRuta(int id)
        {
            var ruta = await _repositorio.GetByIdAsync(id);
            if (ruta == null)
                return NoEncontrado($"No existe la ruta {id}.");

            await _repositorio.RemoveAsync(ruta);
            return NoContent();
        }
    }
}
