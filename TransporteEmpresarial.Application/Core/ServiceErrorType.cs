namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Motivo por el que falló una operación del servicio.
    /// La API lo usa para responder con el código HTTP correcto.
    /// </summary>
    public enum ServiceErrorType
    {
        // Algún campo no cumple sus validaciones (400)
        Validation = 1,

        // El registro solicitado no existe (404)
        NotFound = 2,

        // La operación choca con una regla de negocio o con el estado de los datos (409)
        Conflict = 3,

        // Error inesperado (500)
        Internal = 4
    }
}
