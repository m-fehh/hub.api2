using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Hub.Infrastructure.Database.Models
{
    internal class AccessRule : BaseEntity, IAccessRule
    {
        public override long Id { get; set; }


        [JsonConverter(typeof(ConcreteTypeConverter<AccessRule>))]
        public IAccessRule Parent { get; set; }

        public string Description { get; set; }

        public string KeyName { get; set; }
    }
}
