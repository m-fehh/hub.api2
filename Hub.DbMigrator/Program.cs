﻿using Hub.DbMigrator;
using Hub.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Hub.Infrastructure.MultiTenant.Teste;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        // need to set base path to make it work with shared appsettings files
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTenantSupport(hostContext.Configuration);
        services.AddEntityFrameworkSqlServer<DatabaseContext>();
        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();