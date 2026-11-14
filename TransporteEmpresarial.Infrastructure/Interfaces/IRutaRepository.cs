using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Interfaces
{
    public interface IRutaRepository : IBaseRepository<Ruta>
    {
        /// <summary>
        /// Viajes de la ruta con la unidad y el chofer asignados (trazabilidad de asignaciones).
        /// </summary>
        Task<List<ViajeModel>> GetViajesDeRutaAsync(int rutaId);
    }
}
