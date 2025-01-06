//using Hub.Domain.Administrator.Entities;
//using Hub.Infrastructure.Architecture;
//using Hub.Infrastructure.Architecture.Cache.Interfaces;
//using Hub.Infrastructure.Database.Interfaces;
//using Hub.Infrastructure.Exceptions;
//using Hub.Infrastructure.Web;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;

//namespace Hub.Application.Services.Administrator
//{
//    internal class AppConfigurationService : CrudService<AppConfiguration>
//    {
//        const int TIMEOUT_REDIS = 10; // Minutos
//        private readonly IRedisService _redisService;

//        public AppConfigurationService(IRepository<AppConfiguration> repository, IRedisService redisService) : base(repository)
//        {
//            _redisService = redisService;
//        }

//        public new async Task<long> Insert(AppConfiguration entity)
//        {
//            await ValidateInsert(entity);
//            using (var trans = _repository.BeginTransaction())
//            {
//                var ret = _repository.Insert(entity);

//                if (trans != null) _repository.Commit();

//                return ret;
//            }
//        }

//        public override void Update(AppConfiguration entity)
//        {
//            using (var trans = _repository.BeginTransaction())
//            {
//                _repository.Update(entity);

//                if (trans != null) _repository.Commit();
//            }
//        }

//        /// <summary>
//        /// Retorna um objeto com o a configuração atual do app escolhido
//        /// </summary>
//        /// <param name="appCode"></param>
//        /// <returns></returns>
//        public async Task<AppConfigurationVM> Get(string appCode)
//        {
//            var redisKey = $"AppConfig-{appCode}";

//            var cached = GetCache<AppConfigurationVM>(redisKey);
//            if (cached != null)
//                return cached;

//            var entity = await _repository.Table
//                .Where(r => r.AppCode == appCode)
//                .Select(c => c)
//                .FirstOrDefaultAsync();

//            if (entity != null)
//            {
//                var model = entity.CreateModel();
//                SaveCache(redisKey, model);

//                return model;
//            }

//            return null;
//        }

//        /// <summary>
//        /// Retorna um objeto com o as versões do app escolhido
//        /// </summary>
//        /// <param name="appCode"></param>
//        /// <returns></returns>
//        public async Task<AppVersionVM> GetVersion(string appCode)
//        {
//            var redisKey = $"AppConfigVersion-{appCode}";

//            var cached = GetCache<AppVersionVM>(redisKey);

//            if (cached != null)
//                return cached;

//            var model = await _repository.Table
//                .Where(r => r.AppCode == appCode)
//                .Select(c => c.CreateVersionModel())
//                .FirstOrDefaultAsync();

//            SaveCache(redisKey, model);

//            return model;
//        }

//        public async Task<List<AppConfigurationVM>> GetAll()
//        {
//            var redisKey = $"AppConfiguration-GetAll";

//            var cached = GetCache<List<AppConfigurationVM>>(redisKey);
//            if (cached != null)
//                return cached;

//            var model = await _repository.Table
//                .Select(c => c.CreateModel())
//                .ToListAsync();

//            SaveCache(redisKey, model);

//            return model;
//        }

//        public async Task<AppConfiguration> Create(AppConfigurationVM model)
//        {
//            var existed = await Get(model.AppCode);

//            if (existed != null)
//            {
//                return await Update(model.AppCode, model);
//            }
//            else
//            {
//                var entity = model.CreateEntity();
//                await Insert(entity);

//                return entity;
//            }
//        }

//        public async Task<AppConfiguration> Update(string appCode, AppConfigurationVM model)
//        {
//            var entity = await _repository.Table
//                .Where(r => r.AppCode == appCode)
//                .FirstOrDefaultAsync();

//            if (entity == null)
//            {
//                throw new BusinessException(Engine.Get("AppCodeNotFound"));
//            }

//            entity.Update(model);

//            Update(entity);

//            ClearCache(appCode);

//            return entity;
//        }

//        private async Task ValidateInsert(AppConfiguration entity)
//        {
//            var existing = await Get(entity.AppCode);

//            if (existing != null)
//            {
//                throw new BusinessException(Engine.Get("AppCodeAlreadyExists"));
//            }
//        }

//        private void SaveCache(string redisKey, object model)
//        {
//            _redisService.Set(redisKey, JsonConvert.SerializeObject(model, GetSerializerSettings), TimeSpan.FromMinutes(TIMEOUT_REDIS));
//        }

//        private T GetCache<T>(string redisKey)
//            where T : class
//        {
//            var cached = _redisService.Get(redisKey).ToString();

//            if (!string.IsNullOrEmpty(cached))
//            {
//                return JsonConvert.DeserializeObject<T>(cached, GetSerializerSettings);
//            }

//            return null;
//        }

//        private void ClearCache(string appCode)
//        {
//            var _redisService = Engine.Resolve<IRedisService>();
//            var redisKey = $"AppConfigVersion-{appCode}";

//            _redisService.Delete(redisKey);

//            redisKey = $"AppConfig-{appCode}";

//            _redisService.Delete(redisKey);

//            redisKey = $"AppConfiguration-GetAll";

//            _redisService.Delete(redisKey);
//        }

//        private JsonSerializerSettings GetSerializerSettings => new JsonSerializerSettings
//        {
//            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//        };
//    }



//    public class AppConfigurationVM : AppVersionVM
//    {
//        public virtual string AppCode { get; set; }
//        public virtual string Name { get; set; }
//        public virtual string Environment { get; set; }
//        public virtual string SchemaId { get; set; }
//        public virtual bool Inactive { get; set; }
//        public virtual string DataServerUrl { get; set; }
//        public virtual string ContentServerUrl { get; set; }
//        public virtual string Theme { get; set; }
//        public virtual string Language { get; set; }
//        public virtual string CodePushIOS { get; set; }
//        public virtual string CodePushAndroid { get; set; }
//        public string DataServerAppKeyCode { get; set; }
//        public string ContentServerAppKeyCode { get; set; }
//        public string InvitationUrl { get; set; }
//        public string AppChannel { get; set; }
//        public string ECommerceUrl { get; set; }
//        public string GoogleMapsRestrictionsCountry { get; set; }
//        public string AppChanelStoreLocator { get; set; }
//        public bool? EnableContractSign { get; set; }
//        public string ElosServerApiUrl { get; set; }
//        public string SignalRServerUrl { get; set; }
//        public string DigitalServerUrl { get; set; }
//        public string LogoUrl { get; set; }
//        public string TokenexScriptUrl { get; set; }
//        public string ElosgateStoreTokenUrl { get; set; }
//        public string CheckoutUrl { get; set; }
//        public string CheckoutSupportedVersion { get; set; }
//    }

//    public class AppVersionVM
//    {
//        public virtual string InfoAppStoreIOS { get; set; }
//        public virtual string InfoAppStoreAndroid { get; set; }
//        public virtual string MinVersionIOS { get; set; }
//        public virtual string MinVersionAndroid { get; set; }
//        public virtual string LatestVersionIOS { get; set; }
//        public virtual string LatestVersionAndroid { get; set; }
//    }
//}
