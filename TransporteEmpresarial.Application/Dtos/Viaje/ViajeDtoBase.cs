namespace TransporteEmpresarial.Application.Dtos.Viaje
{
    /// <summary>
    /// Campos para programar o reprogramar un viaje.
    /// La llegada y el turno no se envían: se calculan con la duración de la ruta y la hora de salida.
    /// </summary>
    public abstract class ViajeDtoBase : DtoBase
    {
        public int? RutaId { get; set; }
        public int? VehiculoId { get; set; }
        public int? ConductorId { get; set; }
        public DateTime? FechaHoraSalida { get; set; }

        public override void Normalizar()
        {
            // Los segundos no se usan para programar: 07:30:45 -> 07:30:00
            if (FechaHoraSalida.HasValue)
            {
                var salida = FechaHoraSalida.Value;
                FechaHoraSalida = new DateTime(salida.Year, salida.Month, salida.Day, salida.Hour, salida.Minute, 0, salida.Kind);
            }
        }
    }
}
