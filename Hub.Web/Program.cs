using StackExchange.Redis;
using Autofac;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Autofac.Extensions.DependencyInjection;
using OfficeOpenXml;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Application.Configurations;
using Hub.Infrastructure.Database;
using Hangfire;
using WebEssentials.AspNetCore.OutputCaching;
using Hangfire.Redis.StackExchange;
using Hangfire.MemoryStorage;
using Hangfire.Console;
using Hangfire.Heartbeat;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Hub.Web.Middlewares;
using Hub.Infrastructure.Extensions;
using Hub.Domain.Persistence;
using Hub.Application.Search;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Application.Resource;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.DistributedLock;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Application.Hangfire;
using Hub.Infrastructure.Architecture.Resources;
using Hub.Infrastructure.Web.ModelBinder;
using Hub.Infrastructure.Architecture.Tasks.Interfaces;
using Hub.Infrastructure.Database.Entity;

var builder = WebApplication.CreateBuilder();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

//builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
//{
//    options.Level = (System.IO.Compression.CompressionLevel)CompressionLevel.BestCompression;
//});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

if (builder.Environment.IsDevelopment() == true)
{
    foreach (var item in builder.Configuration.AsEnumerable().Where(c => c.Key.StartsWith("Settings:")))
    {
        Environment.SetEnvironmentVariable(item.Key.Replace("Settings:", ""), item.Value);
    }
}

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    Engine.Initialize(
        executingAssembly: Assembly.GetExecutingAssembly(),
        tasks:
        new List<IStartupTask>()
        {
            new StartupTask()
        },
        dependencyRegistrars: new List<IDependencyConfiguration>
        {
            new DependencyRegistration()
        },
        containerBuilder: builder,
        csb: new ConnectionStringBaseVM()
        {
            ConnectionStringBaseSchema = "sch",
        });
});

LocalizationAttributeBootstrap.Initialize();

builder.Services.AddTenantSupport();
builder.Services.AddEntityFrameworkSqlServer<EntityDbContext>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Engine.AppSettings["ConnectionString-redis"];
    options.InstanceName = Engine.AppSettings["environment"];
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddBrowserDetection();
builder.Services.AddResponseCaching();

builder.Services.AddControllersWithViews(opt =>
{
    opt.ModelMetadataDetailsProviders.Add(new MetadataProvider());
    opt.ModelBinderProviders.Insert(0, new PortlModelBinderProvider());
})
.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());

builder.Services.AddAntiforgery(options => { options.Cookie.Expiration = TimeSpan.Zero; options.SuppressXFrameOptionsHeader = true; });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;


    options.KnownNetworks.Clear(); // Loopback by default, this should be temporary
    options.KnownProxies.Clear(); // Update to include

});

builder.Services.AddOutputCaching();

builder.Services.AddTransient<IOutputCacheVaryByCustomService, OuputCacheVaryByCustomService>();

if (builder.Environment.IsDevelopment() == false)
{
    builder.Services.AddWebOptimizer(
        pipeline =>
        {
            pipeline.MinifyJsFiles("Content/js/**/*.js").ToList().ForEach(p => p.UseContentRoot());
            pipeline.MinifyCssFiles().ToList().ForEach(p => p.UseContentRoot());
        });
}
else
{
    builder.Services.AddWebOptimizer(minifyJavaScript: false, minifyCss: false);
}

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
            new SymmetricSecurityKey(Convert.FromBase64String(Engine.AppSettings["IssuerToken"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["Authentication"];

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

//builder.Services.AddKendo();

builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
}).AddJsonProtocol(options => options.PayloadSerializerOptions.PropertyNamingPolicy = null);

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.KeyLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var hfServerName = Engine.AppSettings["hangfireServerName"];

if (string.IsNullOrEmpty(hfServerName)) hfServerName = "SERVER";

builder.Services
    .AddHangfire(configuration =>
    {
        var hfProvider = Engine.AppSettings["hangfireProvider"];

        switch (hfProvider)
        {
            case "sqlserver":
                configuration.UseSqlServerStorage(Engine.AppSettings["hangfire"],
                    new Hangfire.SqlServer.SqlServerStorageOptions
                    {
                        SchemaName = "hangfire"
                    });

                break;
            case "redis":

                var hangfireRedisDBNumber = Engine.AppSettings["hangfireRedisDBNumber"];

                int.TryParse(hangfireRedisDBNumber, out var dbNumber);

                configuration.UseRedisStorage(
                    ConnectionMultiplexer.Connect(Engine.ConnectionString("hangfire")),
                    new RedisStorageOptions()
                    {
                        Prefix = "hub:{hangfire}:" + hfServerName,
                        Db = dbNumber,
                        ExpiryCheckInterval = TimeSpan.FromMinutes(1),
                        //ajuste para as tarefas não sumam do dashboard do hangfire após 30 minutos
                        InvisibilityTimeout = TimeSpan.FromDays(1)
                    }).WithJobExpirationTimeout(TimeSpan.FromDays(1));
                break;

            case "memory":

                configuration.UseMemoryStorage();

                break;
        }

        configuration.UseConsole();
        configuration.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1));
    });

if (Convert.ToBoolean(Engine.AppSettings["useHangfire"]))
{
    builder.Services
        .AddHangfireServer((options) =>
        {
            options.ServerName = hfServerName;
            options.Queues = new[] { "critical", "default", "low" };
            options.WorkerCount = (Environment.ProcessorCount * 25);
        });

    builder.Services
        .AddHangfireServer((options) =>
        {
            options.ServerName = hfServerName;
            options.Queues = new[] { "limited" };
            options.WorkerCount = 5;
        });

    builder.Services
        .AddHangfireServer((options) =>
        {
            options.ServerName = hfServerName;
            options.Queues = new[] { "invoice" };
            options.WorkerCount = 20;
        });
}

if (Convert.ToBoolean(Engine.AppSettings["UseApiThrottling"]))
{
    //TODO: aplicar throttling
}

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "IAM API",
        Description = "API de integracao com o IAM"
    });
    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http,
        Description = "Basic Authentication using basic scheme"
    });
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.ActionDescriptor.RouteValues["controller"] == "IAM";
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }

            },
            new string[] { }
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var connectionMultiplexer = ConnectionMultiplexer.Connect(Engine.ConnectionString("redis"));

builder.Services.AddDataProtection().PersistKeysToStackExchangeRedis(connectionMultiplexer, $"{(Engine.AppSettings["environment"] ?? "Default").ToUpper()}-DataProtection-Keys");

var app = builder.Build();

app.UseResponseCompression();

//integração do autofac gerenciado pelo asp.net com o framework
//INFO: apenas a partir deste ponto poderemos usar as funções do Engine.
Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "iam-docs";
    c.DocumentTitle = "HUB API - Documentação";
    c.InjectStylesheet("/Content/css/swagger.css");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HUB API");


    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();
app.UseWebOptimizer();
app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Content")),
    RequestPath = "/Content"
});

app.UseRouting();
app.UseCors();

app.UseStatusCodePages(context =>
{

    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized &&
        !request.Path.StartsWithSegments(new PathString("/Api")))
    {
        response.Redirect("/Login");
    }

    return Task.CompletedTask;
});

app.UseRequestCulture();
//app.UsehubRequestTenant();
app.UseRequestAuth();

app.UseOutputCaching();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

//FirebaseConfigurator.Init();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "HUB - Hangfire",
    DisplayStorageConnectionString = false,
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
    IgnoreAntiforgeryToken = true
});

//app.UseEndpoints(endpoints =>
//{
//    RouteConfig.RegisterRoutes(endpoints);

//    endpoints.MapHub<MessagingHub>("/message");

//    endpoints.MapHub<ClientProposalCardHub>("/clientProposalCard");

//    endpoints.MapHub<AttendanceMonitoringDashboardHub>("/attendanceMonitoringDashboard");

//    endpoints.MapHub<AttendanceHub>("/attendance");

//    endpoints.MapHub<SubscriptionAppMessagingHub>("/subscriptionAppMessaging");

//    endpoints.MapHangfireDashboard();
//});

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            if (Engine.AppSettings["LogErrosOnAzureStorageTable"] == "True")
            {
                var ex = contextFeature.Error;
                log4net.LogManager.GetLogger("Sch.SystemInfo").Error(ex.CreateExceptionString(), ex); ;
            }
        }
    });
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    //if (Singleton<MessagingPusher>.Instance != null)
    //{
    //    foreach (var item in Singleton<MessagingPusher>.Instance.InfinityValidationCancelationTokens)
    //    {
    //        item.Cancel();
    //    }
    //}

    Engine.Resolve<IRedisService>().Dispose();
});

var startupTasks = new List<Task>();

startupTasks.Add(Task.Run(() =>
{
    ThreadPoolManager.Configure();
}));

startupTasks.Add(Task.Run(() =>
{
    if (Convert.ToBoolean(Engine.AppSettings["useHangfire"]))
    {
        Engine.Resolve<HangfireStartup>().ConfigureHangfire();
    }
}));

startupTasks.Add(Task.Run(() =>
{
    try
    {
        Engine.Resolve<RedLockManager>().Init();
    }
    catch (Exception ex)
    {
        log4net.LogManager.GetLogger("Sch.SystemInfo").Error("RedLockManager error", ex);
    }
}));

startupTasks.Add(Task.Run(() =>
{
    Engine.Resolve<DefaultLocalizationProvider>().RegisterWrapper(new ResourceWrapper(typeof(TextResource).Assembly, "Resource.TextResource"));
}));

//startupTasks.Add(Task.Run(() =>
//{
//    PageSectionManager.RegisterAllSectionResolvers(new List<IPageSectionResolver>()
//    {
//        new PageActionOrganizationalStructure()
//    });
//}));

startupTasks.Add(Task.Run(() =>
{
    SearchBootstrapper.Initialize(new List<ISearchItem>()
    {
        new SearchDocumentType(),
    });

}));

Task.WaitAll(startupTasks.ToArray());

try
{
    Singleton<LoopTenantManager>.Instance.LoopTenants("SignalRStartup", () =>
    {
        SignalRStartup.Init();
    });
}
catch (Exception ex)
{
    log4net.LogManager.GetLogger("Sch.SystemInfo").Error("SignalRStartup error", ex);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

