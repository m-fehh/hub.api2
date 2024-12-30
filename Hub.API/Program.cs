using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure;
using System.Reflection;
using Hub.Application.Configurations;
using System.Runtime.Intrinsics.X86;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.Extensions;
using Hub.Domain.Persistence;
//using Hub.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

if (builder.Environment.IsDevelopment())
{
    foreach (var item in builder.Configuration.AsEnumerable().Where(c => c.Key.StartsWith("Settings:")))
    {
        Environment.SetEnvironmentVariable(item.Key.Replace("Settings:", ""), item.Value);
    }
}

var configuration = builder.Configuration;

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    Engine.Initialize(
        executingAssembly: Assembly.GetExecutingAssembly(),
        dependencyRegistrars: new List<IDependencyConfiguration>
        {
            new DependencyRegistration()
        },
        containerBuilder: containerBuilder,
        csb: new ConnectionStringBaseVM()
        {
            ConnectionStringBaseSchema = "sch",
        });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

builder.Services.AddTenantSupport();
builder.Services.AddEntityFrameworkSqlServer<EntityDbContext>();

var app = builder.Build();

Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseTenantScopeMiddleware();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
