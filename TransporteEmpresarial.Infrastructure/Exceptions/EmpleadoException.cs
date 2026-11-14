namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    public class EmpleadoException : TransporteException
    {
        public EmpleadoException(string mensaje, TipoError tipo = TipoError.Conflicto) : base(mensaje, tipo) { }
    }
}
