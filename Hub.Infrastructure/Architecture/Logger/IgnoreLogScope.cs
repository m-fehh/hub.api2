namespace Hub.Infrastructure.Architecture.Logger
{
    /// <summary>
    /// Essa classe faz com que os logs de auditoria do repositório não sejam gravados
    /// </summary>
    public class IgnoreLogScope
    {
        public bool Ignore { get => Manager != null && !Manager.Disposed; }

        private IgnoreLogManager Manager = null;

        /// <summary>
        /// Inicia o escopo que faz com que os logs de auditoria do repositório não sejam gravados.
        /// Ao ser encerrado, o sistema voltará a gravar os logs de auditoria.
        /// </summary>
        /// <returns></returns>
        public IDisposable BeginIgnore()
        {
            if (Manager == null || Manager.Disposed)
            {
                Manager = new IgnoreLogManager();

                return Manager;
            }
            else
            {
                return null;
            }
        }
    }

    public class IgnoreLogManager : IDisposable
    {
        public bool Disposed { get; set; }
        public IgnoreLogManager()
        {
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
