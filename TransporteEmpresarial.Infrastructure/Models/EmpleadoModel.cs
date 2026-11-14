using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO de respuesta
    public class EmpleadoModel
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public TipoEmpleado Tipo { get; set; }
        public string? Departamento { get; set; }
        public string? PuntoRecogida { get; set; }
        public string? LicenciaConducir { get; set; }
    }
}
