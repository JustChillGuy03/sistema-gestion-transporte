using TransporteEmpresarial.Domain.Core;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Domain.Entities
{
    /// <summary>
    /// Núcleo del sistema: une una ruta, un vehículo y un conductor en una fecha y hora.
    /// </summary>
    public class Viaje : BaseEntity
    {
        public int RutaId { get; set; }
        public Ruta? Ruta { get; set; }

        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public int ConductorId { get; set; }
        public Empleado? Conductor { get; set; }

        public DateTime FechaHoraSalida { get; set; }

        // Se calcula con la duración estimada de la ruta
        public DateTime FechaHoraLlegada { get; set; }

        public Turno Turno { get; set; }
        public EstadoViaje Estado { get; set; } = EstadoViaje.Programado;

        public ICollection<ViajePasajero> Pasajeros { get; set; } = new List<ViajePasajero>();

        /// <summary>
        /// Turno laboral al que pertenece una hora de salida.
        /// </summary>
        public static Turno TurnoSegunHora(DateTime salida) => salida.Hour switch
        {
            >= 5 and < 12 => Turno.Manana,
            >= 12 and < 18 => Turno.Tarde,
            _ => Turno.Noche
        };
    }
}
