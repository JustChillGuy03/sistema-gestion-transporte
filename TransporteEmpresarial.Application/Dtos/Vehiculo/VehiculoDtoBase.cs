namespace TransporteEmpresarial.Application.Dtos.Vehiculo
{
    /// <summary>
    /// Campos de un vehículo que se envían al registrarlo y al actualizarlo.
    /// </summary>
    public abstract class VehiculoDtoBase : DtoBase
    {
        public string? Placa { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Anio { get; set; }

        // Asientos para pasajeros, sin contar al conductor
        public int? Capacidad { get; set; }

        public override void Normalizar()
        {
            // La placa se guarda en mayúsculas y sin espacios: "a 123456" -> "A123456"
            Placa = Limpiar(Placa)?.Replace(" ", string.Empty).ToUpperInvariant();
            Marca = Limpiar(Marca);
            Modelo = Limpiar(Modelo);
        }
    }
}
