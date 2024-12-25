using System.Configuration;

namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantConfigurationSection : ConfigurationSection
    {
        public string AppPath { get; set; }

        /// <summary>
        /// Coleção de mapeamentos
        /// </summary>
        [ConfigurationProperty("Mapeamentos")]
        [ConfigurationCollection(typeof(NhConfigurationMapeamentoCollection), AddItemName = "MapeamentoNH")]
        public NhConfigurationMapeamentoCollection Mapeamentos
        {
            get { return this["Mapeamentos"] as NhConfigurationMapeamentoCollection; }
            set { this["Mapeamentos"] = value; }
        }
    }


    /// <summary>
    /// Coleção da classe NhConfigurationMapeamento
    /// </summary>
    public class NhConfigurationMapeamentoCollection : ConfigurationElementCollection, IEnumerable<TenantConfigurationMapeamento>
    {
        public TenantConfigurationMapeamento this[int index]
        {
            get
            {
                return base.BaseGet(index) as TenantConfigurationMapeamento;
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

        public TenantConfigurationMapeamento this[object index]
        {
            get
            {
                return base.BaseGet(index) as TenantConfigurationMapeamento;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TenantConfigurationMapeamento();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TenantConfigurationMapeamento)element).MapeamentoId;
        }

        public void Add(TenantConfigurationMapeamento element)
        {
            LockItem = false;  // the workaround
            BaseAdd(element);
        }

        public new IEnumerator<TenantConfigurationMapeamento> GetEnumerator()
        {
            foreach (var key in this.BaseGetAllKeys())
            {
                yield return (TenantConfigurationMapeamento)BaseGet(key);
            }
        }
    }

    /// <summary>
    /// Classe POCO que carrega as informações necessárias para configurar o NHibernate.
    /// </summary>
    public class TenantConfigurationMapeamento : ConfigurationElement
    {
        [ConfigurationProperty("MapeamentoId", IsRequired = true)]
        public string MapeamentoId
        {
            get { return this["MapeamentoId"] as string; }
            set { this["MapeamentoId"] = value; }
        }

        /// <summary>
        /// Coleção de configurações (coleçao da classe NhConfigurationData)
        /// </summary>
        [ConfigurationProperty("Tenants")]
        [ConfigurationCollection(typeof(TenantConfigurationCollection), AddItemName = "ConfigurationTenant")]
        public TenantConfigurationCollection ConfigurationTenants
        {
            get { return this["Tenants"] as TenantConfigurationCollection; }
            set { this["Tenants"] = value; }
        }
    }

    public class TenantConfigurationCollection : ConfigurationElementCollection, IEnumerable<TenantConfigurationData>
    {
        public TenantConfigurationData this[int index]
        {
                get
            {
                    return base.BaseGet(index) as TenantConfigurationData;
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

        public TenantConfigurationData this[object index]
        {
            get
            {
                return base.BaseGet(index) as TenantConfigurationData;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TenantConfigurationData();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TenantConfigurationData)element).TenantId;
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Add(TenantConfigurationData element)
        {
            LockItem = false;  // the workaround
            BaseAdd(element);
        }

        public new IEnumerator<TenantConfigurationData> GetEnumerator()
        {
            foreach (var key in this.BaseGetAllKeys())
            {
                yield return (TenantConfigurationData)BaseGet(key);
            }
        }
    }

    public class TenantConfigurationData : ConfigurationElement, ICloneable
    {
        [ConfigurationProperty("TenantId", IsRequired = true)]
        public string TenantId
        {
            get { return this["TenantId"] as string; }
            set { this["TenantId"] = value; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return this["Name"] as string; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = false)]
        public string ConnectionString
        {
            get { return this["ConnectionString"] as string; }
            set { this["ConnectionString"] = value; }
        }

        [ConfigurationProperty("ConnectionDriver", IsRequired = true)]
        public string ConnectionDriver
        {
            get { return this["ConnectionDriver"] as string; }
            set { this["ConnectionDriver"] = value; }
        }

        [ConfigurationProperty("IsActive", IsRequired = true, DefaultValue = true)]
        public bool IsActive
        {
            get { return (bool)this["IsActive"]; }
            set { this["IsActive"] = value; }
        }

        [ConfigurationProperty("DefaultCulture", IsRequired = false)]
        public string DefaultCulture
        {
            get { return this["DefaultCulture"] as string; }
            set { this["DefaultCulture"] = value; }
        }

        [ConfigurationProperty("SchemaDefault", IsRequired = false)]
        public string SchemaDefault
        {
            get { return this["SchemaDefault"] as string; }
            set { this["SchemaDefault"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
