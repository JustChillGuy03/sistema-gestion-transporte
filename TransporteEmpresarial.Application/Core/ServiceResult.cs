namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Resultado que devuelven todas las operaciones de los servicios.
    /// Indica si la operación tuvo éxito, un mensaje para el usuario y,
    /// si falló, el tipo de error y los errores de validación por campo.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; protected set; } = true;
        public string Message { get; protected set; } = string.Empty;

        // Solo tiene valor cuando Success es false
        public ServiceErrorType? ErrorType { get; protected set; }

        // Errores de validación: nombre del campo -> mensajes
        public IReadOnlyDictionary<string, string[]> Errors { get; protected set; } = new Dictionary<string, string[]>();

        public static ServiceResult Ok(string message) => new() { Message = message };

        public static ServiceResult Fail(string message, ServiceErrorType errorType,
                                         IReadOnlyDictionary<string, string[]>? errors = null)
        {
            var resultado = new ServiceResult();
            resultado.MarcarFallo(message, errorType, errors);
            return resultado;
        }

        protected void MarcarFallo(string message, ServiceErrorType errorType,
                                   IReadOnlyDictionary<string, string[]>? errors)
        {
            Success = false;
            Message = message;
            ErrorType = errorType;
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }

    /// <summary>
    /// Resultado que además trae los datos de la operación (un registro o una lista).
    /// </summary>
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        public static ServiceResult<T> Ok(T data, string message) => new() { Data = data, Message = message };

        public static new ServiceResult<T> Fail(string message, ServiceErrorType errorType,
                                                IReadOnlyDictionary<string, string[]>? errors = null)
        {
            var resultado = new ServiceResult<T>();
            resultado.MarcarFallo(message, errorType, errors);
            return resultado;
        }
    }
}
