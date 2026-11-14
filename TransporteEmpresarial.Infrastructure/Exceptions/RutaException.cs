namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    public class RutaException : TransporteException
    {
        public RutaException(string mensaje, TipoError tipo = TipoError.Conflicto) : base(mensaje, tipo) { }
    }
}
