using Microsoft.AspNetCore.Diagnostics;
using TransporteEmpresarial.Infrastructure.Exceptions;

namespace TransporteEmpresarial.API.Handlers
{
    /// <summary>
    /// Convierte las excepciones de negocio de los repositorios en respuestas HTTP
    /// con formato ProblemDetails. Cualquier otra excepción sigue siendo un 500.
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
