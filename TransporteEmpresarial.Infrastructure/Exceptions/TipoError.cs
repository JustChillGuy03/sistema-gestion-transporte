namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    /// <summary>
    /// Clasifica los errores de negocio para que la API responda con el código HTTP correcto.
    /// </summary>
    public enum TipoError
    {
        // Los datos enviados no son válidos (400)
        Validacion,

        // El registro solicitado no existe (404)
        NoEncontrado,

        // La operación choca con el estado actual de los datos (409)
        Conflicto
    }
}
