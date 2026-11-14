using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Ruta;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Contract
{
    public interface IRutaService : IBaseService<RutaModel, RutaDtoAdd, RutaDtoUpdate>
    {
        /// <summary>
        /// Viajes de la ruta con la unidad y el chofer asignados (trazabilidad de asignaciones).
        /// </summary>
        Task<ServiceResult<List<ViajeModel>>> GetViajesDeRutaAsync(int rutaId);
    }
}
