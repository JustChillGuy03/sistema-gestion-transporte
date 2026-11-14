using TransporteEmpresarial.Application.Core;

namespace TransporteEmpresarial.Application.Exceptions
{
    public class EmpleadoServiceException : ServiceException
    {
        public EmpleadoServiceException(
            string mensaje,
            ServiceErrorType errorType = ServiceErrorType.Validation,
            IReadOnlyDictionary<string, string[]>? errores = null)
            : base(mensaje, errorType, errores) { }
    }
}
