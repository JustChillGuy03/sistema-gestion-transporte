using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // Una línea del historial de un empleado: un viaje en el que fue pasajero o conductor
    public class TrasladoModel
    {
        public int ViajeId { get; set; }
        public TipoEmpleado Rol { get; set; }
        public string Ruta { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraLlegada { get; set; }
        public EstadoViaje Estado { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Conductor { get; set; } = string.Empty;
    }
}
