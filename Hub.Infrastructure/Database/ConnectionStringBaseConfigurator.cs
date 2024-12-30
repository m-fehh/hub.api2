using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Infrastructure.Architecture;

namespace Hub.Infrastructure.Database
{
    public class ConnectionStringBaseConfigurator
    {
        private ConnectionStringBaseVM _model;

        public ConnectionStringBaseConfigurator()
        {
            _model = new ConnectionStringBaseVM()
            {
                ConnectionStringBaseSchema = Engine.AppSettings["ConnectionStringBaseSchema"],
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

    public class ConnectionStringBaseVM
    {
        public string ConnectionStringBaseSchema { get; set; }
    }
}
