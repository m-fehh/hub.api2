namespace Hub.Infrastructure.Database
{
    /// <summary>
    /// Configuração da aplicação relacionada com as configurações para a string de conexão do Entity Framework
    /// </summary>
    public class ConnectionStringBaseConfigurator
    {
        private ConnectionStringBaseVM _model;

        public ConnectionStringBaseConfigurator()
        {
            _model = new ConnectionStringBaseVM()
            {
                //ConnectionStringBaseSchema = Engine.AppSettings["ConnectionStringBaseSchema"],
                //ConnectionString = Engine.AppSettings["ConnectionString"] 
                
                ConnectionStringBaseSchema = "Sch",
                ConnectionString = ""
            };
        }

        public ConnectionStringBaseVM Get()
        {
            return _model;
        }

        public void Set(ConnectionStringBaseVM model)
        {
            _model = model;
        }
    }

    /// <summary>
    /// Classe de modelo que contém as configurações relacionadas à string de conexão do banco de dados.
    /// </summary>
    public class ConnectionStringBaseVM
    {
        public string ConnectionStringBaseSchema { get; set; }
        public string ConnectionString { get; set; }
    }
}
