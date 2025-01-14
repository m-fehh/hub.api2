﻿using Hub.Domain.Entities;
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
using Hub.Application.Services.Enterprise;
using static QRCoder.PayloadGenerator.WiFi;
using System.Text.RegularExpressions;
using Hub.Application.Corporate.Handler;
using Hub.Application.Corporate.Manager;
using Hub.Application.Corporate.Interfaces;

namespace Hub.Application.Services.Users
{
    public class UserService : OrchestratorService<PortalUser>, ISecurityProvider
    {
        public static AsyncLocal<UserContext> CurrentUserContext = new AsyncLocal<UserContext>();
        private const string TOKEN_KEY_USERID = ClaimTypes.NameIdentifier;
        private const string TOKEN_KEY_USERPROFILEID = SystemConstants.TokenKeys.UserProfileId;

        private readonly IRedisService redisService;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly IUserProfileControlAccessService profileControlAccessService;
        private readonly IHubCurrentOrganizationStructure currentOrganizationStructure;
        private readonly IOrgStructBasedService orgStructBasedService;

        public UserService(IRepository<PortalUser> repository, IRedisService redisService, IAccessTokenProvider accessTokenProvider, IUserProfileControlAccessService profileControlAccessService, IHubCurrentOrganizationStructure currentOrganizationStructure, IOrgStructBasedService orgStructBasedService) : base(repository)
        {
            this.redisService = redisService;
            this.accessTokenProvider = accessTokenProvider;
            this.profileControlAccessService = profileControlAccessService;
            this.currentOrganizationStructure = currentOrganizationStructure;
            this.orgStructBasedService = orgStructBasedService;
        }

        private void ValidateInsert(PortalUser user)
        {
            // Verifica se o registro de usuários está permitido pelas configurações da estrutura organizacional
            var allowUserRegistration = bool.Parse(Engine.Resolve<OrganizationalStructureService>().GetCurrentConfigByName("AllowRegisterUser"));

            if (!user.IsFromApi && !allowUserRegistration)
            {
                throw new BusinessException(Engine.Get("NotAllowedChangedBecauseOrgStructConfig"));
            }

            // Valida a entidade base
            Validate(user);

            // Verifica se a senha temporária está presente para autenticação nativa
            if (string.IsNullOrEmpty(user.TempPassword))
            {
                var authProvider = (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), Engine.AppSettings["auth-provider"]);

                if (authProvider == EPortalAuthProvider.Native)
                {
                    throw new BusinessException(user.DefaultRequiredMessage(u => u.TempPassword));
                }
            }
        }


        public override long Insert(PortalUser user)
        {
            if (!user.IsFromApi)
            {
                ValidatePassword(user.Password);
            }

            if (user.DefaultOrgStructure == null && user.OrganizationalStructures != null)
            {
                user.DefaultOrgStructure = user.OrganizationalStructures.FirstOrDefault();
            }

            // Validação de inserção
            ValidateInsert(user);

            // Processa estrutura organizacional do proprietário
            if (!user.IsFromApi)
            {
                orgStructBasedService.LinkOwnerOrgStruct(user);
            }
            else
            {
                user.OwnerOrgStruct = Engine.Resolve<IHubCurrentOrganizationStructure>().GetCurrentRoot();
            }

            using (var transaction = _repository.BeginTransaction())
            {
                user.Person = Engine.Resolve<PersonService>().SavePerson(user.Person.Document, user.Name, user.OrganizationalStructures.ToList(), user.OwnerOrgStruct);

                // Define senha padrão, se não fornecida
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = "Temp#portal!";
                }

                user.Password = user.Password.EncodeSHA1();

                // Gera e atribui palavra-chave
                user.Keyword = Engine.Resolve<UserKeywordService>().GenerateKeyword(user.Name);

                // Insere o usuário no repositório
                var userId = _repository.Insert(user);

                // Configura preferências do usuário
                var userSetting = new PortalUserSetting
                {
                    PortalUserId = user.Id,
                    Name = "current-organizational-structure",
                    Value = user.DefaultOrgStructure.Id.ToString()
                };

                Engine.Resolve<IOrchestratorService<PortalUserSetting>>().Insert(userSetting);

                if (transaction != null) _repository.Commit();

                return userId;
            }
        }


        public override void Update(PortalUser user)
        {
            // Validações do Keyword
            if (string.IsNullOrEmpty(user.Keyword))
            {
                throw new BusinessException(user.DefaultRequiredMessage(e => e.Keyword));
            }

            var keywordService = Engine.Resolve<UserKeywordService>();

            if (!keywordService.IsKeywordValid(user.Keyword))
            {
                throw new BusinessException(Engine.Get("UserKeywordInvalid"));
            }

            if (keywordService.IsKeywordInUse(user.Id, user.Keyword))
            {
                throw new BusinessException(Engine.Get("UserKeywordInUse"));
            }

            // Validação de Estruturas Organizacionais
            if (user.OrganizationalStructures == null || user.OrganizationalStructures.Count == 0)
            {
                throw new BusinessException(Engine.Get("UserOrgStructRequired"));
            }

            // Atribui a Estrutura Organizacional Padrão do Usuário
            SetUserDefaultOrganizationalStructure(user);

            // Atualiza a estrutura organizacional do proprietário
            user.OwnerOrgStructId = _repository.Table.Where(x => x.Id == user.Id).Select(x => x.OwnerOrgStructId).FirstOrDefault();

            // Processa a alteração da senha
            if (user.ChangingPass && !string.IsNullOrEmpty(user.Password))
            {
                user.Password = user.Password.EncodeSHA1();
            }
            else if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = Table.Where(u => u.Id == user.Id).Select(p => p.Password).First();
            }

            // Inicia a transação e realiza as operações
            using (var transaction = _repository.BeginTransaction())
            {
                // Salva as informações pessoais
                user.Person = Engine.Resolve<PersonService>().SavePerson(user.Person.Document, user.Name, user.OrganizationalStructures.ToList());

                _repository.Update(user);

                // Se a senha foi alterada, insere o registro de alteração
                if (user.ChangingPass)
                {
                    InsertPasswordChangeRecord(user);
                }

                if (transaction != null) _repository.Commit();
            }

            // Atualiza a estrutura organizacional do usuário
            currentOrganizationStructure.UpdateUser(user.Id);
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

        public void UpdatePassword(PortalUser entity)
        {
            entity.TempPassword = null;
            entity.Password = entity.Password.EncodeSHA1();

            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Update(entity);

                InsertPasswordChangeRecord(entity);

                if (transaction != null) base._repository.Commit();
            }
        }

        public void SetUserDefaultOrganizationalStructure(PortalUser portalUser)
        {
            var defaultOrgStructureExists = Engine.Resolve<IRepository<PortalUser>>().Table.Where(w => w.Id == portalUser.Id).Any(w => portalUser.OrganizationalStructures.Contains(w.DefaultOrgStructure));

            if (!defaultOrgStructureExists)
            {
                portalUser.DefaultOrgStructure = portalUser.OrganizationalStructures.FirstOrDefault();
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

        public List<long> GetCurrentUserOrgList(PortalUser currentUser = null)
        {
            long? userId = (currentUser != null) ? currentUser.Id : GetCurrentId();

            if (userId == null) return null;

            var userOrgList = redisService.Get($"UserOrgList{userId}").ToString();

            if (string.IsNullOrEmpty(userOrgList))
            {
                var list = Table.Where(c => c.Id == userId).SelectMany(o => o.OrganizationalStructures).Select(o => o.Id).ToList();

                redisService.Set($"UserOrgList{userId}", JsonConvert.SerializeObject(list), TimeSpan.FromHours(4));

                return list;
            }
            else
            {
                return JsonConvert.DeserializeObject<List<long>>(userOrgList);
            }
        }

        public PortalUser CreateTempPassword(string userName)
        {
            var user = base.Table.FirstOrDefault(u => u.Login == userName);

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

        public List<string> GetAuthorizedRoles(List<string> roles)
        {

            var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

            var profile = new List<string>();

            if (!string.IsNullOrEmpty(authCookie))
            {
                var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                accessTokenProvider.ValidateTokenStatus(tokenResult);

                var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERPROFILEID);

                if (!string.IsNullOrWhiteSpace(claim?.Value))
                {
                    var profileId = long.Parse(claim.Value);

                    var profileGroupService = (ProfileGroupService)Engine.Resolve<IOrchestratorService<ProfileGroup>>();
                    profile = profileGroupService.GetAppProfileRoles(profileId).ToList();
                }

            }
            else if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"]))
            {
                var profileGroupService = (ProfileGroupService)Engine.Resolve<IOrchestratorService<ProfileGroup>>();
                profile = profileGroupService.GetAppProfileRoles(1).ToList();
            }

            var values = new List<string>();
            roles.ForEach(role =>
            {
                if (profile.Any(r => r == role || r == "ADMIN"))
                {
                    values.Add(role);
                }
            });

            return values;
        }

        public bool Authorize(string role)
        {
            var authCookie = HttpContextHelper.Current.Request.Cookies["Authentication"];

            if (authCookie != null && string.IsNullOrEmpty(authCookie) == false)
            {
                var tokenResult = accessTokenProvider.ValidateToken(authCookie);

                accessTokenProvider.ValidateTokenStatus(tokenResult);

                var claim = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERPROFILEID);
                var portalUserId = accessTokenProvider.RetriveTokenData(tokenResult, TOKEN_KEY_USERID);

                if (!string.IsNullOrWhiteSpace(portalUserId?.Value))
                {
                    var cacheKey = $"UpdatedUserAccess{portalUserId.Value}";
                    var redisService = Engine.Resolve<IRedisService>();

                    var userToRevoke = redisService.Get(cacheKey).ToString();

                    if (!string.IsNullOrWhiteSpace(userToRevoke))
                    {
                        CookieExtensions.CleanCookies();
                        redisService.Delete(cacheKey);

                        return false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(claim?.Value))
                {
                    var profileId = long.Parse(claim.Value);

                    var profileGroupService = (ProfileGroupService)Engine.Resolve<IOrchestratorService<ProfileGroup>>();

                    return profileGroupService.GetAppProfileRoles(profileId).Any(r => r == role || r == "ADMIN");
                }
            }

            if (bool.Parse(Engine.AppSettings["EnableAnonymousLogin"]))
            {
                var profileGroupService = (ProfileGroupService)Engine.Resolve<IOrchestratorService<ProfileGroup>>();

                return profileGroupService.GetAppProfileRoles(1).Any(r => r == role || r == "ADMIN");
            }

            return false;
        }

        public IUserAccount GetCurrent()
        {
            try
            {
                if (Singleton<OrganizationalHandler>.Instance?.RunningInTestScope ?? false)
                {
                    if (Singleton<OrganizationalScopeManager>.Instance.CurrentUser != null)
                    {
                        return GetById(Singleton<OrganizationalScopeManager>.Instance.CurrentUser.Value);
                    }
                    else
                    {
                        return null;
                    }
                }

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
            catch (Exception)
            {
#if DEBUG
                return _repository.Table.FirstOrDefault();
#endif
                return null;
            }
        }

        public long? GetCurrentId()
        {
            if (Singleton<OrganizationalHandler>.Instance?.RunningInTestScope ?? false)
            {
                return Singleton<OrganizationalScopeManager>.Instance.CurrentUser;
            }

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

        public IProfileGroup GetCurrentProfile()
        {
            try
            {
                if (Singleton<OrganizationalHandler>.Instance?.RunningInTestScope ?? false)
                {
                    if (Singleton<OrganizationalScopeManager>.Instance.CurrentUser != null)
                    {
                        var userId = Singleton<OrganizationalScopeManager>.Instance.CurrentUser.Value;

                        return Table.Where(u => u.Id == userId).Select(u => u.Profile).FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }

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
            catch (Exception)
            {
#if DEBUG
                return _repository.Table.FirstOrDefault().Profile;
#endif
                throw;
            }
        }

        public long? GetCurrentProfileId()
        {
            try
            {
                if (Singleton<OrganizationalHandler>.Instance?.RunningInTestScope ?? false)
                {
                    if (Singleton<OrganizationalScopeManager>.Instance.CurrentUser != null)
                    {
                        return GetById(Singleton<OrganizationalScopeManager>.Instance.CurrentUser.Value).Profile.Id;
                    }
                    else
                    {
                        return null;
                    }
                }

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
            catch (Exception)
            {
#if DEBUG
                return 1;
#endif
                throw;
            }
        }

        public override PortalUser GetById(long id)
        {
            var user = Engine.Resolve<IRepository<PortalUser>>().Table.Include(u => u.Profile).FirstOrDefault(u => u.Id == id);
            return user;
        }

        public void ValidatePassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                if (password.Length < 8 || password.Length > 30 || Regex.Match(password, @"\d+").Success == false || Regex.Match(password, @"[a-z]").Success == false || Regex.Match(password, @"[A-Z]").Success == false || password.All(c => Char.IsLetterOrDigit(c)) == true)
                {
                    throw new BusinessException(Engine.Get("PasswordToWeak"));
                }
            }
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

                                qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(logo));
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

        /// <summary>
        /// Método responsável por inativar todos os usuários que possuam data de expiração
        /// e que esta data seja igual ou inferior à data atual
        /// <see cref=">https://dev.azure.com/evuptec/Kanban%20EL/_workitems/edit/24571"/> Link da Feature
        /// </summary>
        public void InactivateExpiredUsers()
        {
            var userExpiredList = Table.Where(w => w.ExpirationDate.Value.Date <= DateTime.Now.Date).ToList();

            foreach (var userExpired in userExpiredList)
            {
                userExpired.ExpirationDate = null;
                userExpired.Inactive = true;
                Update(userExpired);
            }
        }

        #region PRIVATE METHODS 

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

        private void InsertPasswordChangeRecord(PortalUser entity)
        {
            var passwordExpirationDays = Engine.Resolve<ProfileGroupService>().Get(w => w.Id == entity.Profile.Id, s => s.PasswordExpirationDays).FirstOrDefault();
            var passHistory = new PortalUserPassHistory
            {
                PortalUserId = entity.Id,
                PortalUser = entity,
                CreationUTC = DateTime.UtcNow,
                Password = entity.Password
            };

            if (passwordExpirationDays != EPasswordExpirationDays.Off)
            {
                passHistory.ExpirationUTC = passHistory.CreationUTC.AddDays((double)passwordExpirationDays);
            }

            Engine.Resolve<PortalUserPassHistoryService>().Insert(passHistory);
        }

        /// <summary>
        /// Método responsável por registrar o acesso do usuário no portal
        /// </summary>
        public void RegisterAccess(long? userId = null)
        {
            long? currentUserId = userId;

            if (currentUserId == null)
            {
                currentUserId = Engine.Resolve<ISecurityProvider>().GetCurrentId();
            }

            PortalUser user = GetById(currentUserId.Value);
            user.TempPassword = null;
            user.LastAccessDate = DateTime.Now;
            user.LastUpdateUTC = DateTime.UtcNow;

            Update(user);
        }

        #endregion

        #region AUTHENTICATION  

        public AuthResultVM AuthenticateToken(string token)
        {
            var tokenResult = Engine.Resolve<IAccessTokenProvider>().ValidateToken(token);

            if (tokenResult.Status != AccessTokenStatus.Valid)
            {
                return null;
            }

            var claim = tokenResult.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return null;
            }

            var id = long.Parse(claim.Value);

            var user = GetById(id);

            var orgs = GetCurrentUserOrgList(user);

            //var establishments = Engine.Resolve<IRepository<Establishment>>().Table.Where(e => orgs.Contains(e.OrganizationalStructure.Id)).Select(e => e.CNPJ).ToList();

            return new AuthResultVM
            {
                Id = user.QrCodeInfo,
                CPF = user.Document,
                Name = user.Name,
                Email = user.Email,
                MobilePhone = user.AreaCode + user.PhoneNumber,
                Inactive = user.Inactive,
                ProfileId = user.Profile.Id,
                ProfileName = user.Profile.Name,
                //Establishments = establishments
            };
        }

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
                    Infrastructure.Extensions.CookieExtensions.CleanCookies();
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
            var currentUser = base.Table.FirstOrDefault(u => u.Login == userName && u.TempPassword == tempPassword);

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

            return (model.Token != null);
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

                Engine.Resolve<LogService>().Audit(new LogAuditVM
                {
                    ObjectName = Engine.Get("Login"),
                    ObjectId = portalUserId,
                    Message = request.FingerPrint?.ToString()
                });

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
