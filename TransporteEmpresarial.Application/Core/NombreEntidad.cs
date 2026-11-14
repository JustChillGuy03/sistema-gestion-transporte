namespace TransporteEmpresarial.Application.Core
{
    /// <summary>
    /// Nombre de la entidad que maneja un servicio, para armar los mensajes
    /// con el artículo y el género correctos ("la ruta", "el vehículo", "registrada", "registrado").
    /// </summary>
    public sealed record NombreEntidad(string Singular, string Plural, bool Femenino)
    {
        public string Titulo => char.ToUpper(Singular[0]) + Singular[1..];

        // "la ruta" / "el vehículo"
        public string ConArticulo => $"{(Femenino ? "la" : "el")} {Singular}";

        // "de la ruta" / "del vehículo"
        public string DeArticulo => Femenino ? $"de la {Singular}" : $"del {Singular}";

        // "las rutas" / "los vehículos"
        public string PluralConArticulo => $"{(Femenino ? "las" : "los")} {Plural}";

        // Terminación de los participios: registrada / registrado
        public string Terminacion => Femenino ? "a" : "o";
    }
}
