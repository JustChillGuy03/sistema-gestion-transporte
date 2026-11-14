namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    public class ViajeException : TransporteException
    {
        public ViajeException(string mensaje, TipoError tipo = TipoError.Conflicto) : base(mensaje, tipo) { }
    }
}
