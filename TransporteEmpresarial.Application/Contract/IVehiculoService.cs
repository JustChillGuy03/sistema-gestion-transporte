using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Vehiculo;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Contract
{
    public interface IVehiculoService : IBaseService<VehiculoModel, VehiculoDtoAdd, VehiculoDtoUpdate>
    {
        /// <summary>
        /// Vehículos en estado Disponible que no tienen viajes vigentes en el intervalo [desde, hasta).
        /// </summary>
        Task<ServiceResult<List<VehiculoModel>>> GetDisponiblesAsync(DateTime? desde, DateTime? hasta);
    }
}
