using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO de respuesta: incluye los datos de la ruta, la unidad y el chofer asignados
    public class ViajeModel
    {
        public int Id { get; set; }
        public int RutaId { get; set; }
        public string Ruta { get; set; } = string.Empty;
        public int VehiculoId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int ConductorId { get; set; }
        public string Conductor { get; set; } = string.Empty;
        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraLlegada { get; set; }
        public Turno Turno { get; set; }
        public EstadoViaje Estado { get; set; }
        public int Capacidad { get; set; }
        public int PasajerosInscritos { get; set; }
        public int CuposDisponibles => Capacidad - PasajerosInscritos;
    }
}
