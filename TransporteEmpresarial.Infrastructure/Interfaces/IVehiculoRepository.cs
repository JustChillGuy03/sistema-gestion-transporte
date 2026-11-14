using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Repository;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Interfaces
{
    public interface IVehiculoRepository : IBaseRepository<Vehiculo>
    {
        /// <summary>
        /// Vehículos en estado Disponible que no tienen ningún viaje activo
        /// que se cruce con el intervalo [desde, hasta).
        /// </summary>
        Task<List<VehiculoModel>> GetDisponiblesAsync(DateTime desde, DateTime hasta);
    }
}
