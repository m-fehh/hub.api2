using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hub.Infrastructure.Database.Models
{
    internal class UserAccount : BaseEntity, IUserAccount
    {
        public override long Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string IpAddress { get; set; }


        [JsonConverter(typeof(ConcreteTypeConverter<ProfileGroup>))]
        public IProfileGroup Profile { get; set; }


        public long? DefaultOrgStructureId { get; set; }

        public List<long> OrgStructures { get; set; }
    }
}
