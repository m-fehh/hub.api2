using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Hub.Infrastructure.Database.Models
{
    internal class ProfileGroup : BaseEntity, IProfileGroup
    {
        public override long Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(ConcreteListTypeConverter<IAccessRule, AccessRule>))]
        public ICollection<IAccessRule> Rules { get; set; }

        public bool Administrator { get; set; }
    }
}
