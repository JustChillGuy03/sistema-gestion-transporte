using TransporteEmpresarial.Domain.Core;

namespace TransporteEmpresarial.Domain.Entities
{
    public class Ruta : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;

        // Paradas en orden entre el origen y el destino
        public List<string> ParadasIntermedias { get; set; } = new();

        public int DuracionEstimadaMinutos { get; set; }

        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
    }
}
