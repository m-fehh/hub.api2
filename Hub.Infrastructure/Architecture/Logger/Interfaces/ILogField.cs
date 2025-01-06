using Hub.Infrastructure.Database.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Infrastructure.Architecture.Logger.Interfaces
{
    public interface ILogField : IBaseEntity
    {
        ILog Log { get; set; }

        string PropertyName { get; set; }

        string OldValue { get; set; }

        string NewValue { get; set; }

        ISet<ILog> Childs { get; set; }
    }
}
