using TransporteEmpresarial.Application.Core;

namespace TransporteEmpresarial.Application.Exceptions
{
    public class ViajeServiceException : ServiceException
    {
        public ViajeServiceException(
            string mensaje,
            ServiceErrorType errorType = ServiceErrorType.Validation,
            IReadOnlyDictionary<string, string[]>? errores = null)
            : base(mensaje, errorType, errores) { }
    }
}
