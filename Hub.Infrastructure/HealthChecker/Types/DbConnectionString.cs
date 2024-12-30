using Hub.Infrastructure.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Infrastructure.HealthChecker.Types
{
    public class DbConnectionString : SingleBase<string>
    {
        public DbConnectionString() { }

        public DbConnectionString(string value) : base(value) { }
    }
}
