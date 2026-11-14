using System.ComponentModel.DataAnnotations;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para actualizar un empleado. El tipo (Pasajero/Conductor) no se cambia después de registrarlo.
    public class EmpleadoUpdateModel
    {
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "La cédula debe tener el formato 000-0000000-0.")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(60)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(60)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100)]
        public string? Correo { get; set; }

        [StringLength(60)]
        public string? Departamento { get; set; }

        [StringLength(150)]
        public string? PuntoRecogida { get; set; }

        [StringLength(20)]
        public string? LicenciaConducir { get; set; }
    }
}
