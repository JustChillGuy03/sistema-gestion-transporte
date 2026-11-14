using Microsoft.AspNetCore.Mvc;

namespace TransporteEmpresarial.API.Controllers
{
    /// <summary>
    /// Respuestas de error comunes con el mismo formato que usa TransporteExceptionHandler.
    /// </summary>
    public abstract class TransporteControllerBase : ControllerBase
    {
        protected ObjectResult NoEncontrado(string detalle) =>
            Problem(statusCode: StatusCodes.Status404NotFound, title: "Recurso no encontrado", detail: detalle);

        protected ObjectResult DatosNoValidos(string detalle) =>
            Problem(statusCode: StatusCodes.Status400BadRequest, title: "Datos no válidos", detail: detalle);
    }
}
