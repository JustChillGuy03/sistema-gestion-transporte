using TransporteEmpresarial.Application.Core;

namespace TransporteEmpresarial.Application.Exceptions
{
    /// <summary>
    /// Base de las excepciones de la capa de servicios. Hereda de Exception.
    /// Los servicios la lanzan cuando un dato no es válido o un registro no existe,
    /// y BaseService la convierte en un ServiceResult fallido.
    /// </summary>
    public abstract class ServiceException : Exception
    {
        public ServiceErrorType ErrorType { get; }

        // Errores de validación por campo (vacío si el error no es de validación)
        public IReadOnlyDictionary<string, string[]> Errores { get; }

        protected ServiceException(string mensaje, ServiceErrorType errorType,
                                   IReadOnlyDictionary<string, string[]>? errores = null)
            : base(mensaje)
        {
            ErrorType = errorType;
            Errores = errores ?? new Dictionary<string, string[]>();
        }
    }
}
