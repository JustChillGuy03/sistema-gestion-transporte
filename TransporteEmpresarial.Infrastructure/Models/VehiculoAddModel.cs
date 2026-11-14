using System.ComponentModel.DataAnnotations;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para registrar un vehículo (entra siempre como Disponible)
    public class VehiculoAddModel
    {
        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "La placa debe tener entre 5 y 10 caracteres.")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Range(1990, 2100, ErrorMessage = "El año debe estar entre 1990 y 2100.")]
        public int Anio { get; set; }

        [Range(1, 60, ErrorMessage = "La capacidad debe estar entre 1 y 60 asientos.")]
        public int Capacidad { get; set; }
    }
}
