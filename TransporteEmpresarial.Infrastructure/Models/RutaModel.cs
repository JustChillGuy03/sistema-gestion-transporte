namespace TransporteEmpresarial.Infrastructure.Models
{
    // DTO de respuesta
    public class RutaModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public List<string> ParadasIntermedias { get; set; } = new();
        public int DuracionEstimadaMinutos { get; set; }
    }
}
