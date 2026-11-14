using Microsoft.AspNetCore.Mvc;
using TransporteEmpresarial.Application.Core;

namespace TransporteEmpresarial.API.Controllers
{
    /// <summary>
    /// Convierte el ServiceResult que devuelven los servicios en la respuesta HTTP:
    /// éxito -> 200/201/204 con los datos; fallo -> ProblemDetails con 400, 404, 409 o 500.
    /// </summary>
    public abstract class TransporteControllerBase : ControllerBase
    {
        protected ActionResult Responder<T>(ServiceResult<T> resultado) =>
            resultado.Success ? Ok(resultado.Data) : Error(resultado);

        protected ActionResult ResponderSinContenido(ServiceResult resultado) =>
            resultado.Success ? NoContent() : Error(resultado);

        protected ActionResult Error(ServiceResult resultado)
        {
            // Errores de validación por campo: mismo formato que usa ASP.NET Core (ValidationProblemDetails)
            if (resultado.Errors.Count > 0)
            {
                foreach (var (campo, mensajes) in resultado.Errors)
                    foreach (var mensaje in mensajes)
                        ModelState.AddModelError(campo, mensaje);

                return ValidationProblem(detail: resultado.Message, title: "Datos no válidos");
            }

            var (codigo, titulo) = resultado.ErrorType switch
            {
                ServiceErrorType.Validation => (StatusCodes.Status400BadRequest, "Datos no válidos"),
                ServiceErrorType.NotFound => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ServiceErrorType.Conflict => (StatusCodes.Status409Conflict, "Regla de negocio no cumplida"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno")
            };

            return Problem(statusCode: codigo, title: titulo, detail: resultado.Message);
        }
    }
}
