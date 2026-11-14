using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Application.Dtos.Viaje
{
    // DTO para cambiar el estado de un viaje: EnCurso, Completado o Cancelado
    public class ViajeDtoCambiarEstado : DtoBase
    {
        public EstadoViaje? Estado { get; set; }
    }
}
