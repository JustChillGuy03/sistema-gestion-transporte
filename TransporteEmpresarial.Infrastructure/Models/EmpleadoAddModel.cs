using System.ComponentModel.DataAnnotations;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para registrar un empleado: los mismos datos de actualización más el tipo
    public class EmpleadoAddModel : EmpleadoUpdateModel
    {
        [Required(ErrorMessage = "El tipo es obligatorio (Pasajero o Conductor).")]
        public TipoEmpleado? Tipo { get; set; }
    }
}
