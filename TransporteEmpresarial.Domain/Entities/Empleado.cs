using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Domain.Entities
{
    public class Empleado : Persona
    {
        public TipoEmpleado Tipo { get; set; }
        public string? Departamento { get; set; }

        // Solo aplica a pasajeros: lugar donde el transporte lo recoge
        public string? PuntoRecogida { get; set; }

        // Solo aplica a conductores (es obligatoria para ellos)
        public string? LicenciaConducir { get; set; }

        // Viajes que manejó o tiene asignados como conductor
        public ICollection<Viaje> ViajesComoConductor { get; set; } = new List<Viaje>();

        // Viajes en los que está inscrito como pasajero
        public ICollection<ViajePasajero> Traslados { get; set; } = new List<ViajePasajero>();
    }
}
