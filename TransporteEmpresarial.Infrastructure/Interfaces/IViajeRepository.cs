using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Interfaces
{
    public interface IViajeRepository : IBaseRepository<Viaje>
    {
        Task<List<ViajeModel>> GetViajesAsync(DateTime? fecha, EstadoViaje? estado);
        Task<ViajeModel?> GetViajeAsync(int id);
        Task<List<PasajeroModel>> GetPasajerosAsync(int viajeId);

        // Valida el cupo del vehículo y que el pasajero no tenga otro viaje a la misma hora
        Task InscribirPasajeroAsync(int viajeId, int empleadoId);
        Task QuitarPasajeroAsync(int viajeId, int empleadoId);

        // Programado -> EnCurso -> Completado; Programado/EnCurso -> Cancelado
        Task CambiarEstadoAsync(int viajeId, EstadoViaje nuevoEstado);
    }
}
