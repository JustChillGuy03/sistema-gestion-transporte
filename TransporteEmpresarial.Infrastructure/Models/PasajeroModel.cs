namespace TransporteEmpresarial.Infrastructure.Models
{
    // Pasajero inscrito en un viaje
    public class PasajeroModel
    {
        public int EmpleadoId { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? PuntoRecogida { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
