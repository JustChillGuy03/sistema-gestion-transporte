namespace TransporteEmpresarial.Domain.Core
{
    /// <summary>
    /// Datos personales comunes. Empleado hereda de esta clase.
    /// </summary>
    public abstract class Persona : BaseEntity
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
