using TransporteEmpresarial.Application.Core;

namespace TransporteEmpresarial.Application.Exceptions
{
    public class VehiculoServiceException : ServiceException
    {
        public VehiculoServiceException(
            string mensaje,
            ServiceErrorType errorType = ServiceErrorType.Validation,
            IReadOnlyDictionary<string, string[]>? errores = null)
            : base(mensaje, errorType, errores) { }
    }
}
