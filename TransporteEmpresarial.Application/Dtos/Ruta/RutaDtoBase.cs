namespace TransporteEmpresarial.Application.Dtos.Ruta
{
    /// <summary>
    /// Campos de una ruta que se envían al registrarla y al actualizarla.
    /// Son anulables para que el servicio detecte los que faltan y lo indique por campo.
    /// </summary>
    public abstract class RutaDtoBase : DtoBase
    {
        public string? Nombre { get; set; }
        public string? Origen { get; set; }
        public string? Destino { get; set; }

        // Paradas en orden entre el origen y el destino (opcional)
        public List<string>? ParadasIntermedias { get; set; }

        public int? DuracionEstimadaMinutos { get; set; }

        public override void Normalizar()
        {
            Nombre = Limpiar(Nombre);
            Origen = Limpiar(Origen);
            Destino = Limpiar(Destino);

            // Se quitan los espacios de cada parada; las vacías se conservan para avisar del error
            ParadasIntermedias = ParadasIntermedias?.Select(p => p?.Trim() ?? string.Empty).ToList();
        }
    }
}
