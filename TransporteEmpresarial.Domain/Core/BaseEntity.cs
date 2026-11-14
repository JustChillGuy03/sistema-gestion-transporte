namespace TransporteEmpresarial.Domain.Core
{
    /// <summary>
    /// Campos que comparten todas las entidades: identificador, auditoría y borrado lógico.
    /// Las fechas de creación y modificación las asigna el contexto al guardar.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Borrado lógico: el registro no se elimina de la tabla para no perder el historial
        public bool Eliminado { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }
}
