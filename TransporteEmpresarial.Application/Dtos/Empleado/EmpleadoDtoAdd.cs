using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Application.Dtos.Empleado
{
    // DTO para registrar un empleado: sus datos más el tipo (Pasajero o Conductor)
    public class EmpleadoDtoAdd : EmpleadoDtoBase
    {
        public TipoEmpleado? Tipo { get; set; }
    }
}
