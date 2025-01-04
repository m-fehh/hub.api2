using Hub.Application.Models.ViewModels.Auth;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.OAuth.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Hub.Application.Services
{
    public class LoginService
    {
        private readonly IAccessTokenProvider _tokenService;

        public LoginService(IAccessTokenProvider tokenService)
        {
            _tokenService = tokenService;
        }

        public string AuthenticateUser(LoginVM model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                throw new BusinessException(Engine.Get("UsernameCannotBeNull"));
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                throw new BusinessException(Engine.Get("PasswordCannotBeNull"));
            }

            return GenerateUserToken(model);

        }

        string GenerateUserToken(LoginVM model)
        {
            var password = model.Password.EncodeSHA1();
            var user = Engine.Resolve<IRepository<PortalUser>>().Table.FirstOrDefault(x => x.Login == model.Username && x.Password == password);

            if (user == null)
            {
                throw new BusinessException(Engine.Get("InvalidLogin"));
            }

            if (user.Inactive)
            {
                throw new BusinessException(Engine.Get("UserInactive"));
            }

            var lastChangePassword = Engine.Resolve<IRepository<PortalUserPassHistory>>().Table.Where(x => x.PortalUserId == user.Id).Select(x => x.ExpirationUTC).FirstOrDefault();

            if (lastChangePassword.HasValue && lastChangePassword.Value < DateTime.UtcNow)
            {
                throw new BusinessException(Engine.Get("PasswordExpired"));
            }

            Engine.Resolve<UserService>().RegisterAccess(user.Id);

            return CreateTokenForUser(user);
        }

        string CreateTokenForUser(PortalUser model)
        {

            var redis = Engine.Resolve<IRedisService>();
            string cacheKey = $"UpdatedUserAccess{model.Id}";

            var userToRevoke = redis.Get(cacheKey).ToString();

            if (!string.IsNullOrWhiteSpace(userToRevoke))
            {
                redis.Delete(cacheKey);
            }

            redis.Set($"UserOrgList{model.Id}", JsonConvert.SerializeObject(model.OrganizationalStructures), TimeSpan.FromDays(7));

            double tokenExpirationTimeInMinutes = double.TryParse(Engine.AppSettings["auth-token-expiration-time"], out var tokenExpirationTime) ? tokenExpirationTime : 180;

            var token = _tokenService.GenerateToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                        new Claim(ClaimTypes.Name, model.Name),
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim("admin", model.Profile?.Administrator.ToString()),
                        new Claim("profileId", model.ProfileId.ToString()),
                        new Claim("defaultOrgStructureId", model.DefaultOrgStructureId?.ToString() ?? ""),
                    }, expiryInMinutes: tokenExpirationTimeInMinutes);

            return token;
        }
    }
}
