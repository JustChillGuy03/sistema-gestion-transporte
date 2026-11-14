using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Application.Dtos.Vehiculo
{
    // DTO para actualizar un vehículo: además de sus datos permite cambiar el estado
    public class VehiculoDtoUpdate : VehiculoDtoBase
    {
        public EstadoVehiculo? Estado { get; set; }
    }
}
