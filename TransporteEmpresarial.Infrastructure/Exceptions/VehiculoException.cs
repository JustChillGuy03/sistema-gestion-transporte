namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    public class VehiculoException : TransporteException
    {
        public VehiculoException(string mensaje, TipoError tipo = TipoError.Conflicto) : base(mensaje, tipo) { }
    }
}
