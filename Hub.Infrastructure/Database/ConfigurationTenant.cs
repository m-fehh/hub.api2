using System.Configuration;

namespace Hub.Infrastructure.Database
{
    public class ConfigurationTenant : ConfigurationSection
    {
        /// <summary>
        /// Local da aplicação. Utilizado para que localize as dlls que serão serializadas e terão as entidades reconhecidas.
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// Coleção de mapeamentos
        /// </summary>
        [ConfigurationProperty("Mapeamentos")]
        [ConfigurationCollection(typeof(ConfigurationMapeamentoCollection), AddItemName = "Mapeamento")]
        public ConfigurationMapeamentoCollection Mapeamentos
        {
            get { return this["Mapeamentos"] as ConfigurationMapeamentoCollection; }
            set { this["Mapeamentos"] = value; }
        }
    }

    /// <summary>
    /// Coleção da classe ConfigurationMapeamento
    /// </summary>
    public class ConfigurationMapeamentoCollection : ConfigurationElementCollection, IEnumerable<ConfigurationMapeamento>
    {
        public ConfigurationMapeamento this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConfigurationMapeamento;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public ConfigurationMapeamento this[object index]
        {
            get
            {
                return base.BaseGet(index) as ConfigurationMapeamento;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationMapeamento();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationMapeamento)element).MapeamentoId;
        }

        public void Add(ConfigurationMapeamento element)
        {
            LockItem = false;  // the workaround
            BaseAdd(element);
        }

        public new IEnumerator<ConfigurationMapeamento> GetEnumerator()
        {
            foreach (var key in this.BaseGetAllKeys())
            {
                yield return (ConfigurationMapeamento)BaseGet(key);
            }
        }
    }

    /// <summary>
    /// Classe POCO que carrega as informações necessárias para configurar o NHibernate.
    /// </summary>
    public class ConfigurationMapeamento : ConfigurationElement
    {
        [ConfigurationProperty("MapeamentoId", IsRequired = true)]
        public string MapeamentoId
        {
            get { return this["MapeamentoId"] as string; }
            set { this["MapeamentoId"] = value; }
        }

        /// <summary>
        /// Coleção de configurações (coleçao da classe ConfigurationData)
        /// </summary>
        [ConfigurationProperty("Tenants")]
        [ConfigurationCollection(typeof(ConfigurationDataCollection), AddItemName = "ConfigurationTenant")]
        public ConfigurationDataCollection ConfigurationTenants
        {
            get { return this["Tenants"] as ConfigurationDataCollection; }
            set { this["Tenants"] = value; }
        }
    }

    /// <summary>
    /// Coleção da classe ConfigurationData
    /// </summary>
    public class ConfigurationDataCollection : ConfigurationElementCollection, IEnumerable<ConfigurationData>
    {
        public ConfigurationData this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConfigurationData;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public ConfigurationData this[object index]
        {
            get
            {
                return base.BaseGet(index) as ConfigurationData;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationData();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationData)element).TenantId;
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Add(ConfigurationData element)
        {
            LockItem = false;  // the workaround
            BaseAdd(element);
        }

        public new IEnumerator<ConfigurationData> GetEnumerator()
        {
            foreach (var key in this.BaseGetAllKeys())
            {
                yield return (ConfigurationData)BaseGet(key);
            }
        }
    }

    /// <summary>
    /// Classe POCO que carrega as informações necessárias para configurar o NHibernate.
    /// </summary>
    public class ConfigurationData : ConfigurationElement, ICloneable
    {
        [ConfigurationProperty("TenantId", IsRequired = true)]
        public long TenantId
        {
            get { return (long)this["TenantId"]; }  // Conversão explícita
            set { this["TenantId"] = value; }
        }

        [ConfigurationProperty("TenantName", IsRequired = true)]
        public string TenantName
        {
            get { return this["TenantName"] as string; }
            set { this["TenantName"] = value; }
        }

        [ConfigurationProperty("Subdomain", IsRequired = true)]
        public string Subdomain
        {
            get { return this["Subdomain"] as string; }
            set { this["Subdomain"] = value; }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return this["ConnectionString"] as string; }
            set { this["ConnectionString"] = value; }
        }

        [ConfigurationProperty("SchemaDefault")]
        public string SchemaDefault
        {
            get { return this["SchemaDefault"] as string; }
            set { this["SchemaDefault"] = value; }
        }

        /// <summary>
        /// Propriedade para armazenar a cultura padrão do tenant.
        /// </summary>
        [ConfigurationProperty("Culture", IsRequired = false)]
        public string Culture
        {
            get { return this["Culture"] as string; }
            set { this["Culture"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
