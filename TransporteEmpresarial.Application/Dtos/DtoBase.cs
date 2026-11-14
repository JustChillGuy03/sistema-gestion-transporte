namespace TransporteEmpresarial.Application.Dtos
{
    /// <summary>
    /// Base de todos los DTOs de entrada de la capa de servicios.
    /// Antes de validar, el servicio llama a Normalizar() para quitar espacios
    /// sobrantes y unificar formatos, de modo que se valida y se guarda el mismo valor.
    /// </summary>
    public abstract class DtoBase
    {
        public virtual void Normalizar() { }

        // Texto sin espacios al inicio ni al final; null si viene vacío
        protected static string? Limpiar(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
