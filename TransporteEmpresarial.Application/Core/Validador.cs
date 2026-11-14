using System.Text.RegularExpressions;

namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Acumula los errores de validación de un DTO, agrupados por campo,
    /// para devolverlos todos juntos en lugar de uno a la vez.
    /// </summary>
    public sealed class Validador
    {
        private readonly Dictionary<string, List<string>> _errores = new();

        public bool EsValido => _errores.Count == 0;

        public IReadOnlyDictionary<string, string[]> Errores =>
            _errores.ToDictionary(e => e.Key, e => e.Value.ToArray());

        public bool TieneErrorEn(string campo) => _errores.ContainsKey(campo);

        public Validador Agregar(string campo, string mensaje)
        {
            if (!_errores.TryGetValue(campo, out var mensajes))
            {
                mensajes = new List<string>();
                _errores[campo] = mensajes;
            }

            mensajes.Add(mensaje);
            return this;
        }

        /// <summary>
        /// Agrega el error si la condición NO se cumple.
        /// </summary>
        public Validador Regla(bool condicion, string campo, string mensaje)
        {
            if (!condicion)
                Agregar(campo, mensaje);

            return this;
        }

        // ---------------- Textos ----------------

        public Validador Requerido(string campo, string? valor, string? nombreVisible = null)
        {
            if (string.IsNullOrWhiteSpace(valor))
                Agregar(campo, $"El campo '{nombreVisible ?? campo}' es obligatorio.");

            return this;
        }

        public Validador TextoObligatorio(string campo, string? valor, int minimo, int maximo, string? nombreVisible = null)
        {
            var nombre = nombreVisible ?? campo;

            if (string.IsNullOrWhiteSpace(valor))
                return Agregar(campo, $"El campo '{nombre}' es obligatorio.");

            return Longitud(campo, valor, minimo, maximo, nombre);
        }

        /// <summary>
        /// Si el texto viene vacío no hay error; si viene, debe respetar la longitud.
        /// </summary>
        public Validador TextoOpcional(string campo, string? valor, int minimo, int maximo, string? nombreVisible = null)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return this;

            return Longitud(campo, valor, minimo, maximo, nombreVisible ?? campo);
        }

        /// <summary>
        /// Valida el formato solo si el campo tiene valor y no tiene ya otro error
        /// (así no se repiten mensajes del mismo campo).
        /// </summary>
        public Validador Formato(string campo, string? valor, Regex patron, string mensaje)
        {
            if (!string.IsNullOrWhiteSpace(valor) && !TieneErrorEn(campo) && !patron.IsMatch(valor))
                Agregar(campo, mensaje);

            return this;
        }

        /// <summary>
        /// El campo debe venir vacío (por ejemplo, un dato que no aplica a ese tipo de empleado).
        /// </summary>
        public Validador Vacio(string campo, string? valor, string mensaje)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                Agregar(campo, mensaje);

            return this;
        }

        private Validador Longitud(string campo, string valor, int minimo, int maximo, string nombre)
        {
            var largo = valor.Trim().Length;

            if (largo < minimo || largo > maximo)
            {
                var mensaje = minimo == maximo
                    ? $"El campo '{nombre}' debe tener {maximo} caracteres."
                    : $"El campo '{nombre}' debe tener entre {minimo} y {maximo} caracteres.";
                Agregar(campo, mensaje);
            }

            return this;
        }

        // ---------------- Números, fechas y enums ----------------

        public Validador Obligatorio<T>(string campo, T? valor, string? nombreVisible = null) where T : struct
        {
            if (!valor.HasValue)
                Agregar(campo, $"El campo '{nombreVisible ?? campo}' es obligatorio.");

            return this;
        }

        public Validador Rango(string campo, int? valor, int minimo, int maximo, string? nombreVisible = null)
        {
            var nombre = nombreVisible ?? campo;

            if (!valor.HasValue)
                return Agregar(campo, $"El campo '{nombre}' es obligatorio.");

            if (valor < minimo || valor > maximo)
                Agregar(campo, $"El campo '{nombre}' debe estar entre {minimo} y {maximo}.");

            return this;
        }

        /// <summary>
        /// Identificador de otro registro (ruta, vehículo, empleado...): obligatorio y mayor que cero.
        /// </summary>
        public Validador Identificador(string campo, int? valor, string? nombreVisible = null)
        {
            var nombre = nombreVisible ?? campo;

            if (!valor.HasValue)
                return Agregar(campo, $"El campo '{nombre}' es obligatorio.");

            if (valor <= 0)
                Agregar(campo, $"El campo '{nombre}' debe ser un número mayor que cero.");

            return this;
        }

        public Validador EnumValido<TEnum>(string campo, TEnum? valor, string? nombreVisible = null) where TEnum : struct, Enum
        {
            var nombre = nombreVisible ?? campo;
            var permitidos = string.Join(", ", Enum.GetNames<TEnum>());

            if (!valor.HasValue)
                return Agregar(campo, $"El campo '{nombre}' es obligatorio ({permitidos}).");

            if (!Enum.IsDefined(valor.Value))
                Agregar(campo, $"El valor de '{nombre}' no es válido. Valores permitidos: {permitidos}.");

            return this;
        }
    }
}
