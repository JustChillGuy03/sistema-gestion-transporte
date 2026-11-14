using System.ComponentModel.DataAnnotations;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para programar un viaje. La llegada y el turno se calculan a partir de la salida.
    public class ViajeAddModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Indique una ruta válida.")]
        public int RutaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Indique un vehículo válido.")]
        public int VehiculoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Indique un conductor válido.")]
        public int ConductorId { get; set; }

        [Required(ErrorMessage = "La fecha y hora de salida es obligatoria.")]
        public DateTime? FechaHoraSalida { get; set; }
    }
}
