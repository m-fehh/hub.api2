using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.OAuth.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Extensions;
using System.Security.Claims;

namespace Hub.Application.Services
{
    public class LoginService
    {
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


            user.LastAccessDate = DateTime.Now;
            Engine.Resolve<IRepository<PortalUser>>().Update(user);

            return CreateTokenForUser(new LoginResponseVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ProfileId = user.ProfileId,
                Administrator = user.Profile.Administrator,
                Inactive = user.Inactive
            });
        }

        string CreateTokenForUser(LoginResponseVM user)
        {
            double tokenExpirationTimeInMinutes = double.TryParse(Engine.AppSettings["auth-token-expiration-time"], out var tokenExpirationTime) ? tokenExpirationTime : 180;

            var token = Engine.Resolve<IAccessTokenProvider>().GenerateToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("admin", user.Administrator.ToString()),
                        new Claim("profileId", user.ProfileId.ToString())
                    }, expiryInMinutes: tokenExpirationTimeInMinutes);

            return token;
        }
    }

    public class LoginVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseVM
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long? ProfileId { get; set; }
        public bool Administrator { get; set; }
        public bool Inactive { get; set; }
        public long? DefaultOrgStructureId { get; set; }
        public bool AllowMultipleAccess { get; set; }
    }
}
