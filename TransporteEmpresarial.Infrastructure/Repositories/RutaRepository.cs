using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Infrastructure.Context;
using TransporteEmpresarial.Infrastructure.Core;
using TransporteEmpresarial.Infrastructure.Exceptions;
using TransporteEmpresarial.Infrastructure.Extensions;
using TransporteEmpresarial.Infrastructure.Interfaces;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Repositories
{
    public class RutaRepository : BaseRepository<Ruta>, IRutaRepository
    {
        public RutaRepository(TransporteContext context) : base(context) { }

        public override async Task AddAsync(Ruta ruta)
        {
            await ValidarNombreUnicoAsync(ruta);
            await base.AddAsync(ruta);
        }

        // Cambiar la duración no modifica la llegada de los viajes ya programados;
        // se recalcula cuando un viaje se reprograma.
        public override async Task UpdateAsync(Ruta ruta)
        {
            await ValidarNombreUnicoAsync(ruta);
            await base.UpdateAsync(ruta);
        }

        public override async Task RemoveAsync(Ruta ruta)
        {
            if (await _context.Viajes.Vigentes().AnyAsync(v => v.RutaId == ruta.Id))
                throw new RutaException($"No se puede eliminar la ruta \"{ruta.Nombre}\": tiene viajes programados o en curso.");

            await base.RemoveAsync(ruta);
        }

        public async Task<List<ViajeModel>> GetViajesDeRutaAsync(int rutaId)
        {
            if (!await ExistsAsync(r => r.Id == rutaId))
                throw new RutaException($"No existe la ruta {rutaId}.", TipoError.NoEncontrado);

            return await _context.Viajes.AsNoTracking()
                                        .Where(v => !v.Eliminado && v.RutaId == rutaId)
                                        .OrderByDescending(v => v.FechaHoraSalida)
                                        .ProyectarAModelo()
                                        .ToListAsync();
        }

        private async Task ValidarNombreUnicoAsync(Ruta ruta)
        {
            if (await ExistsAsync(r => r.Nombre == ruta.Nombre && r.Id != ruta.Id))
                throw new RutaException($"Ya existe una ruta con el nombre \"{ruta.Nombre}\".");
        }
    }
}
