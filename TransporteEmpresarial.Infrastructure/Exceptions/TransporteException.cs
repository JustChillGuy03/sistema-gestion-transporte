namespace TransporteEmpresarial.Infrastructure.Exceptions
{
    /// <summary>
    /// Base de las excepciones de negocio que lanzan los repositorios.
    /// </summary>
    public abstract class TransporteException : Exception
    {
        public TipoError Tipo { get; }

        protected TransporteException(string mensaje, TipoError tipo) : base(mensaje)
        {
            Tipo = tipo;
        }
    }
}
