using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Viaje;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Contract
{
    public interface IViajeService : IBaseService<ViajeModel, ViajeDtoAdd, ViajeDtoUpdate>
    {
        // Filtros opcionales por día de salida y por estado
        Task<ServiceResult<List<ViajeModel>>> GetViajesAsync(DateTime? fecha, EstadoViaje? estado);

        Task<ServiceResult<List<PasajeroModel>>> GetPasajerosAsync(int viajeId);

        // Valida el cupo del vehículo y que el pasajero no tenga otro viaje a la misma hora
        Task<ServiceResult<ViajeModel>> InscribirPasajeroAsync(int viajeId, int empleadoId);
        Task<ServiceResult> QuitarPasajeroAsync(int viajeId, int empleadoId);

        // Programado -> EnCurso -> Completado; Programado/EnCurso -> Cancelado
        Task<ServiceResult<ViajeModel>> CambiarEstadoAsync(int viajeId, ViajeDtoCambiarEstado dto);
    }
}
