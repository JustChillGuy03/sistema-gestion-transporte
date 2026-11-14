using Microsoft.AspNetCore.Diagnostics;
using TransporteEmpresarial.Infrastructure.Exceptions;

namespace TransporteEmpresarial.API.Handlers
{
    /// <summary>
    /// Respaldo: convierte en ProblemDetails (400/404/409) una excepción de negocio de los
    /// repositorios que llegue a la API sin pasar por un servicio. Los servicios ya las
    /// capturan y devuelven un ServiceResult. Cualquier otra excepción sigue siendo un 500.
    /// </summary>
    public sealed class TransporteExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public TransporteExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not TransporteException error)
                return false;

            var (codigo, titulo) = error.Tipo switch
            {
                TipoError.Validacion => (StatusCodes.Status400BadRequest, "Datos no válidos"),
                TipoError.NoEncontrado => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                _ => (StatusCodes.Status409Conflict, "Regla de negocio no cumplida")
            };

            httpContext.Response.StatusCode = codigo;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails =
                {
                    Status = codigo,
                    Title = titulo,
                    Detail = error.Message
                }
            });
        }
    }
}
