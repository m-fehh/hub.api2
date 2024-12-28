﻿using Hub.Infrastructure.Database.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        public const string DefaultSchemaName = "dbo";
        private readonly string _defaultConnectionString;
        private readonly Dictionary<string, string> _connectionStringPerTenant;

        public string? CurrentTenant => TenantContext.CurrentTenant;

        public string DbSchemaName => CurrentTenant ?? DefaultSchemaName;

        public string ConnectionString
        {
            get
            {
                if (CurrentTenant != null && _connectionStringPerTenant.TryGetValue(CurrentTenant, out var connectionString))
                {
                    return connectionString;
                }

                return Engine.ConnectionString("default");
            }
        }

        public TenantProvider(IOptions<TenantConfigurationOptions> tenantConfigurationOptions)
        {
            _connectionStringPerTenant = tenantConfigurationOptions.Value.Tenants
                .Where(t => !string.IsNullOrWhiteSpace(t.ConnectionString))
                .ToDictionary(ks => ks.Name, vs => vs.ConnectionString!);
        }

        public IDisposable BeginScope(string tenant)
        {
            return TenantContext.BeginScope(tenant);
        }

        public override string? ToString()
        {
            return CurrentTenant;
        }
    }
}