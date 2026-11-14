using System.ComponentModel.DataAnnotations;

namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO para registrar una ruta
    public class RutaAddModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El origen es obligatorio.")]
        [StringLength(150)]
        public string Origen { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destino es obligatorio.")]
        [StringLength(150)]
        public string Destino { get; set; } = string.Empty;

        public List<string> ParadasIntermedias { get; set; } = new();

        [Range(5, 600, ErrorMessage = "La duración estimada debe estar entre 5 y 600 minutos.")]
        public int DuracionEstimadaMinutos { get; set; }
    }
}
