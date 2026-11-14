using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Domain.Entities
{
    public class Vehiculo : BaseEntity
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Anio { get; set; }

        // Asientos disponibles para pasajeros (sin contar al conductor)
        public int Capacidad { get; set; }

        public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.Disponible;

        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
    }
}
