using System.ComponentModel.DataAnnotations;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    public class CambiarEstadoViajeModel
    {
        [Required(ErrorMessage = "El estado es obligatorio (EnCurso, Completado o Cancelado).")]
        public EstadoViaje? Estado { get; set; }
    }
}
