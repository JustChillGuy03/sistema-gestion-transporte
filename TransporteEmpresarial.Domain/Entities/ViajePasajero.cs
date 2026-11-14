namespace TransporteEmpresarial.Domain.Entities
{
    /// <summary>
    /// Inscripción de un empleado (pasajero) en un viaje.
    /// Guarda la lista de pasajeros y sirve como historial de traslados.
    /// </summary>
    public class ViajePasajero
    {
        public int ViajeId { get; set; }
        public Viaje? Viaje { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        public DateTime FechaInscripcion { get; set; }
    }
}
