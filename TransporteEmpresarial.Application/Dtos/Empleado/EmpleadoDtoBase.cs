using System.Text.RegularExpressions;

namespace TransporteEmpresarial.Application.Dtos.Empleado
{
    /// <summary>
    /// Campos de un empleado que se envían al registrarlo y al actualizarlo.
    /// </summary>
    public abstract class EmpleadoDtoBase : DtoBase
    {
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Departamento { get; set; }

        // Solo para pasajeros: lugar donde el transporte lo recoge
        public string? PuntoRecogida { get; set; }

        // Solo para conductores
        public string? LicenciaConducir { get; set; }

        public override void Normalizar()
        {
            Cedula = Limpiar(Cedula);

            // Si la cédula llega sin guiones (11 dígitos) se le da el formato 000-0000000-0
            if (Cedula is not null && Regex.IsMatch(Cedula, @"^\d{11}$"))
                Cedula = $"{Cedula[..3]}-{Cedula[3..10]}-{Cedula[10]}";

            Nombre = Limpiar(Nombre);
            Apellido = Limpiar(Apellido);
            Telefono = Limpiar(Telefono);
            Correo = Limpiar(Correo)?.ToLowerInvariant();
            Departamento = Limpiar(Departamento);
            PuntoRecogida = Limpiar(PuntoRecogida);
            LicenciaConducir = Limpiar(LicenciaConducir)?.ToUpperInvariant();
        }
    }
}
