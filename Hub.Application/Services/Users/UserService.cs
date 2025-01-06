using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Architecture.OAuth.Interfaces;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;
using System.Security.Claims;
using Hub.Infrastructure.Generator;
using System.Drawing;
using System.Net;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Web.Interfaces;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Hub.Application.Models.Helpers;
using Hub.Infrastructure.Architecture.OAuth;
using Hub.Domain.Entities.Users;
using Hub.Application.Corporate.Configurations;
using Hub.Application.Models.ViewModels;
using Hub.Domain.Enums;
using Hub.Infrastructure.Database.Models.Helpers;

namespace Hub.Application.Services.Users
{
    public class UserService : OrchestratorService<PortalUser>, ISecurityProvider
    {
        public static AsyncLocal<UserContext> CurrentUserContext = new AsyncLocal<UserContext>();
        private const string TOKEN_KEY_USERID = ClaimTypes.NameIdentifier;
        private const string TOKEN_KEY_USERPROFILEID = SystemConstants.TokenKeys.UserProfileId;

        private readonly IRedisService redisService;
        private readonly ITenantManager tenantManager;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly IUserProfileControlAccessService profileControlAccessService;

        public UserService(IRepository<PortalUser> repository, IRedisService redisService, ITenantManager tenantManager, IAccessTokenProvider accessTokenProvider, IUserProfileControlAccessService profileControlAccessService) : base(repository)
        {
            this.redisService = redisService;
            this.tenantManager = tenantManager;
            this.accessTokenProvider = accessTokenProvider;
            this.profileControlAccessService = profileControlAccessService;
        }

        private void Validate(PortalUser entity)
        {
            var profileRepository = Engine.Resolve<IRepository<ProfileGroup>>();

            var isAdmin = profileRepository.Table.Where(p => p.Id == entity.Profile.Id).Select(p => p.Administrator).FirstOrDefault();

            //apenas um admin pode manipular outro
            if (entity.Id != 0 && _repository.Table.Where(u => u.Id == entity.Id).Select(u => u.Profile.Administrator).FirstOrDefault())
            {
                var currentUser = Engine.Resolve<ISecurityProvider>().GetCurrentId();

                if (currentUser != null)
                {
                    if (!_repository.Table.Where(u => u.Id == currentUser).Select(u => u.Profile.Administrator).FirstOrDefault())
                    {
                        throw new BusinessException(Engine.Get("OnlyAdminCanChangeAdminUser"));
                    }
                }
            }

            if (Table.Any(u => u.Login == entity.Login && u.Id != entity.Id))
            {
                throw new BusinessException(entity.DefaultAlreadyRegisteredMessage(e => e.Login));
            }


            //Realiza validações na troca de Senha
            if (entity.ChangingPass)
            {
                ValidateRecentPasswords(entity);
            }
        }

        private void ValidateRecentPasswords(PortalUser entity)
        {
            if (entity.Password.Length < 8)
            {
                throw new BusinessException(Engine.Get("ErrorPasswordLength"));
            }

            var lastPasswords = Engine.Resolve<PortalUserPassHistoryService>().Get(w => w.PortalUser.Id == entity.Id,
                                                          s => new PortalUserPassHistory
                                                          {
                                                              Password = s.Password,
                                                              CreationUTC = s.CreationUTC
                                                          }).OrderByDescending(o => o.CreationUTC)
                                                          .Take(3)
                                                          .Select(s => s.Password)
                                                          .ToList();

            var password = entity.Password.EncodeSHA1();

            if (lastPasswords.Contains(password))
            {
                throw new BusinessException(Engine.Get("PasswordAlreadyUsed"));
            }
        }

        public PortalUser ResetPassword(string document, string newPassword)
        {
            var user = Table.FirstOrDefault(u => u.Person.Document.Equals(document));

            if (user != null && !string.IsNullOrEmpty(newPassword))
            {
                user.TempPassword = newPassword;
                user.LastPasswordRecoverRequestDate = DateTime.Now;

                _repository.Update(user);

                return user;
            }

            return null;
        }

        public override void Delete(long id)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                var entity = GetById(id);
                _repository.Delete(id);

                if (transaction != null) _repository.Commit();
            }
        }

        public PortalUser CreateTempPassword(string userName)
        {
            var user = Table.FirstOrDefault(u => u.Login == userName);

            if (user != null)
            {
                user.TempPassword = PasswordGeneration.Generate(6, 1);
                user.LastPasswordRecoverRequestDate = DateTime.Now;

                Update(user);

                return user;
            }

            return null;
        }

        public void SetCurrentUser(string token)
        {
            CurrentUserContext.Value.AuthToken = token;
        }

        public IUserAccount GetCurrent()
        {
            try
            {
                if (CurrentUserContext.Value?.CurrentUserId != null)
                {
                    var userId = CurrentUserContext.Value?.CurrentUserId;
                    return GetById(userId.Value);
                }
                else
                {
                    if (HttpContextHelper.Current == null || HttpContextHelper.Current.Request == null) return null;

                    var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

                    if (!string.IsNullOrEmpty(authCookie))
                    {
                        var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                        accessTokenProvider.ValidateTokenStatus(tokenResult);

                        var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERID);

                        if (!string.IsNullOrWhiteSpace(claim?.Value))
                        {
                            var userId = long.Parse(claim.Value);

                            return GetById(userId);
                        }
                    }

                    if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"]))
                    {
                        return GetById(1);
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public long? GetCurrentId()
        {
            if (CurrentUserContext.Value?.CurrentUserId != null)
            {
                return CurrentUserContext.Value?.CurrentUserId;
            }
            else
            {
                if (HttpContextHelper.Current == null || HttpContextHelper.Current.Request == null) return null;

                var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

                if (!string.IsNullOrEmpty(authCookie))
                {
                    var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                    accessTokenProvider.ValidateTokenStatus(tokenResult);

                    var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERID);

                    if (!string.IsNullOrWhiteSpace(claim?.Value))
                    {
                        var userId = long.Parse(claim.Value);

                        return userId;
                    }
                }

                if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"] ?? "false"))
                {
                    return 1;
                }

                return null;
            }
        }

        public long? GetCurrentProfileId()
        {
            try
            {
                if (CurrentUserContext.Value?.CurrentUserId != null)
                {
                    var userId = CurrentUserContext.Value?.CurrentUserId;
                    return Table.Where(u => u.Id == userId).Select(u => u.Profile.Id).FirstOrDefault();
                }
                else
                {
                    if (HttpContextHelper.Current == null || HttpContextHelper.Current.Request == null) return null;

                    var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

                    if (!string.IsNullOrEmpty(authCookie))
                    {
                        var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                        accessTokenProvider.ValidateTokenStatus(tokenResult);

                        var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERPROFILEID);

                        if (!string.IsNullOrWhiteSpace(claim?.Value))
                        {
                            var profileId = long.Parse(claim.Value);

                            return profileId;
                        }
                    }

                    if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"]))
                    {
                        return 1;
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IProfileGroup GetCurrentProfile()
        {
            try
            {
                if (CurrentUserContext.Value?.CurrentUserId != null)
                {
                    var userId = CurrentUserContext.Value?.CurrentUserId;

                    return Table.Where(u => u.Id == userId).Select(u => u.Profile).FirstOrDefault();
                }
                else
                {
                    if (HttpContextHelper.Current == null || HttpContextHelper.Current.Request == null) return null;

                    var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

                    if (!string.IsNullOrEmpty(authCookie))
                    {
                        var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                        accessTokenProvider.ValidateTokenStatus(tokenResult);

                        var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERPROFILEID);

                        if (!string.IsNullOrWhiteSpace(claim?.Value))
                        {
                            var profileId = long.Parse(claim.Value);

                            return Engine.Resolve<IRepository<ProfileGroup>>().GetById(profileId);
                        }
                    }

                    if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"]))
                    {
                        return Engine.Resolve<IRepository<ProfileGroup>>().GetById(1);
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public override PortalUser GetById(long id)
        {
            var user = Engine.Resolve<IRepository<PortalUser>>().Table.Include(u => u.Profile).FirstOrDefault(u => u.Id == id);
            return user;
        }

        public async Task<byte[]> GenerateQRCode(string qrCodeInfo, string logo = null)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeInfo, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        Bitmap qrCodeImage = qrCode.GetGraphic(13);

                        if (!string.IsNullOrEmpty(logo))
                        {
                            if (logo.StartsWith("~/"))
                            {
                                logo = logo.Substring(2);

                                qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Image.FromFile(logo));
                            }

                            if (logo.Contains("http"))
                            {
                                var request = (HttpWebRequest)WebRequest.Create(logo);
                                request.Method = "GET";

                                using (HttpClient httpClient = new HttpClient())
                                {
                                    using (HttpResponseMessage response = await httpClient.GetAsync(logo))
                                    {
                                        if (response.IsSuccessStatusCode)
                                        {
                                            using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                                            {
                                                qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Image.FromStream(responseStream));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        using (var stream = new MemoryStream())
                        {
                            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                            return stream.ToArray();
                        }
                    }
                }
            }
        }


        #region AUTHENTICATION  


        public void Authenticate(string token)
        {
            var accessService = Engine.Resolve<IAccessTokenProvider>();
            var tokenResult = accessService.ValidateExternalToken(token, Engine.AppSettings["etrust-secret-key"]);

            PortalUser currentUser = null;

            if (tokenResult != null)
            {
                HttpContextHelper.Current.Response.Cookies.Append("jsession", token);

                accessService.ValidateTokenStatus(tokenResult);
                var jwtHandler = new JwtSecurityTokenHandler();
                var tokenRead = jwtHandler.ReadJwtToken(token);
                var tokenUser = tokenRead.Claims.Where(w => w.Type.Equals("user")).FirstOrDefault()?.Value;

                if (string.IsNullOrEmpty(tokenUser) == false)
                {
                    var userJwtTokenKeys = JsonConvert.DeserializeObject<UserJwtTokenKeys>(tokenUser);

                    var userIdClaim = userJwtTokenKeys.UserId;
                    var userEmailClaim = userJwtTokenKeys.UserEmail;
                    var userDocClaim = userJwtTokenKeys.UserDoc;

                    var portalUserTable = Engine.Resolve<IOrchestratorService<PortalUser>>().Table;

                    if (currentUser == null && string.IsNullOrWhiteSpace(userIdClaim) == false)
                    {
                        var userId = long.Parse(userIdClaim);

                        currentUser = portalUserTable.FirstOrDefault(f => f.Id == userId && f.Inactive == false);
                    }

                    if (currentUser == null && string.IsNullOrWhiteSpace(userEmailClaim) == false)
                    {
                        var userEmail = long.Parse(userEmailClaim);
                        currentUser = portalUserTable.FirstOrDefault(w => w.Email.Equals(userEmail) && w.Inactive == false);
                    }

                    if (currentUser == null && string.IsNullOrWhiteSpace(userDocClaim) == false)
                    {
                        currentUser = portalUserTable.FirstOrDefault(w => w.Person.Document.Equals(userDocClaim) && w.Inactive == false);
                    }
                }

                if (currentUser == null)
                {
                    CookieExtensions.CleanCookies();
                    HttpContextHelper.Current.Response.Redirect("~/Login", false);
                }

                var authData = new AuthUserToken { UserId = currentUser.Id, UserProfileId = currentUser.Profile.Id };

                var newToken = GenerateJWT(authData);

                HttpContextHelper.Current.Response.Cookies.Append("Authentication", newToken);

                CurrentUserContext.Value.AuthToken = newToken;
                CurrentUserContext.Value.CurrentUserId = currentUser.Id;

                var parameter = new ProfileControlAccess(currentUser.Id, newToken);

                profileControlAccessService.Save(parameter, ((ProfileGroup)currentUser.Profile).AllowMultipleAccess);
            }
        }

        public bool AuthenticateTemp(string userName, string tempPassword)
        {
            var currentUser = Table.FirstOrDefault(u => u.Login == userName && u.TempPassword == tempPassword);

            if (currentUser != null)
            {
                if (currentUser.Inactive)
                {
                    throw new BusinessException(Engine.Get("InactiveUser"));
                }

                return true;
            }
            return false;
        }


        public bool Authenticate(AuthDetails authenticationVM)
        {
            var model = new LoginCredentialsVM()
            {
                Login = authenticationVM.UserName,
                Password = authenticationVM.Password,
                RememberMe = authenticationVM.RememberMe,
                Provider = EAuthProvider.Form,
                FingerPrint = (FingerPrintVM)authenticationVM.FingerPrint
            };

            Authenticate(model);

            return model.Token != null;
        }

        public void Authenticate(LoginCredentialsVM request)
        {
            try
            {
                var token = Engine.Resolve<LoginService>().AuthenticateUser(new LoginVM { Username = request.Login, Password = request.Password });

                var tokenResult = Engine.Resolve<IAccessTokenProvider>().ValidateToken(token);

                if (tokenResult.Status != AccessTokenStatus.Valid)
                {
                    throw new BusinessException(Engine.Get("UsernameInvalid"));
                }

                var portalUserId = long.Parse(accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERID).Value);

                using (var trans = _repository.BeginTransaction())
                {

                    if (request.FingerPrint != null)
                    {
                        var portalUserFingerprint = new PortalUserFingerprint()
                        {
                            PortalUser = new PortalUser() { Id = portalUserId },
                            OS = request.FingerPrint.OS,
                            BrowserName = request.FingerPrint.BrowserName,
                            BrowserInfo = request.FingerPrint.BrowserInfo,
                            IpAddress = request.FingerPrint.IpAddress
                        };
                        Engine.Resolve<PortalUserFingerprintService>().Insert(portalUserFingerprint);
                    }

                    if (trans != null) _repository.Commit();
                }

                //Engine.Resolve<LogService>().Audit(new Log.Models.LogAuditVM
                //{
                //    ObjectName = Engine.Get("Login"),
                //    ObjectId = portalUserId,
                //    Message = request.FingerPrint?.ToString()
                //});

                if (request.Provider == EAuthProvider.Form)
                {
                    HttpContextHelper.Current.Response.Cookies.Append("Authentication", token);

                    CurrentUserContext.Value.AuthToken = token;
                    CurrentUserContext.Value.CurrentUserId = portalUserId;

                    var parameter = new ProfileControlAccess(portalUserId, token);

                    var allowMultipleAccess = bool.Parse(accessTokenProvider.RetriveTokenData(tokenResult, "allowMultipleAccess")?.Value ?? "True");

                    profileControlAccessService.Save(parameter, allowMultipleAccess);
                }
                else
                {
                    token = GenerateJWT(portalUserId);
                }

                request.Token = token;

            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
        }

        #endregion

        #region JWT GENERATOR 

        public string GenerateJWT(long? userId = null)
        {
            if (userId == null) userId = GetCurrentId();
            return accessTokenProvider.GenerateToken(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, expiryInMinutes: 1);
        }

        public string GenerateJWT(AuthUserToken tokenData)
        {
            var tokenExpirationTimeInMinutes = double.Parse(Engine.AppSettings["auth-token-expiration-time"]);

            return accessTokenProvider.GenerateToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, tokenData.UserId.ToString()),
                        new Claim(TOKEN_KEY_USERID, tokenData.UserId.ToString()),
                        new Claim(TOKEN_KEY_USERPROFILEID, tokenData.UserProfileId.ToString())
                    }, expiryInMinutes: tokenExpirationTimeInMinutes);
        }

        #endregion
    }
}
