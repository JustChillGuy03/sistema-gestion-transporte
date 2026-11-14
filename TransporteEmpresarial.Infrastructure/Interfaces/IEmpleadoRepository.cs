using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Interfaces
{
    public interface IEmpleadoRepository : IBaseRepository<Empleado>
    {
        Task<List<EmpleadoModel>> GetPorTipoAsync(TipoEmpleado tipo);

        /// <summary>
        /// Historial de viajes del empleado, como pasajero o como conductor, del más reciente al más antiguo.
        /// </summary>
        Task<List<TrasladoModel>> GetHistorialAsync(int empleadoId);
    }
}
