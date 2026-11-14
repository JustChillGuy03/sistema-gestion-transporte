using System.ComponentModel.DataAnnotations;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para actualizar un vehículo: además de los datos permite cambiar su estado
    public class VehiculoUpdateModel : VehiculoAddModel
    {
        [Required(ErrorMessage = "El estado es obligatorio (Disponible, EnMantenimiento o FueraDeServicio).")]
        public EstadoVehiculo? Estado { get; set; }
    }
}
