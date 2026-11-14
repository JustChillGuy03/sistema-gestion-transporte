using TransporteEmpresarial.Application.Core;
using TransporteEmpresarial.Application.Dtos.Empleado;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Application.Contract
{
    public interface IEmpleadoService : IBaseService<EmpleadoModel, EmpleadoDtoAdd, EmpleadoDtoUpdate>
    {
        Task<ServiceResult<List<EmpleadoModel>>> GetPorTipoAsync(TipoEmpleado tipo);

        /// <summary>
        /// Historial de viajes del empleado, como pasajero o como conductor.
        /// </summary>
        Task<ServiceResult<List<TrasladoModel>>> GetHistorialAsync(int empleadoId);
    }
}
