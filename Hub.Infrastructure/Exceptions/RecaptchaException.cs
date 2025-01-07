using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Exceptions;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Hub.Infrastructure.Exceptions
{
    public class RecaptchaException : BusinessException
    {
        public RecaptchaException(string message) : base(message) { }
    }

    public class RecaptchaValidationRequest
    {
        public string Token { get; set; }
        public string Version { get; set; }
    }

}

//public class GoogleRecaptchaService
//{
//    /// <summary>
//    /// Método responsável por validar a chave recaptcha enviada pela camada de UI (normalmente usada na tela de login)
//    /// </summary>
//    /// <param name="token">chave recaptcha enviada pelo google para o componente do recaptcha da UI</param>
//    public void Validate(string token)
//    {
//        this.Validate(new RecaptchaValidationRequest() { Token = token, Version = "3" });
//    }

//    /// <summary>
//    /// Método responsável por validar a chave recaptcha enviada pela camada de UI (normalmente usada na tela de login)
//    /// </summary>
//    /// <param name="token">chave recaptcha enviada pelo google para o componente do recaptcha da UI</param>
//    public void Validate(RecaptchaValidationRequest validationRequest)
//    {
//        if (validationRequest.Version == "3")
//        {
//            ValidateV3(validationRequest.Token);
//        }
//        else if (validationRequest.Version == "2")
//        {
//            ValidateV2(validationRequest.Token);
//        }
//        else
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }
//    }

//    private static void ValidateV3(string recaptchaClientResponse)
//    {
//        var recaptchaServerToken = Engine.AppSettings["google-recaptcha-server"];

//        if (string.IsNullOrEmpty(recaptchaServerToken)) return;

//        if (string.IsNullOrEmpty(recaptchaClientResponse))
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }

//        var response = Engine.Resolve<ApiRequestService>().MakeRequest(
//            new RequestParameters()
//            {
//                APIBaseAddress = "https://www.google.com",
//                APIMethodAddress = "/recaptcha/api/siteverify",
//                Method = RestSharp.Method.POST,
//                ParameterType = RestSharp.ParameterType.QueryString,
//                Parameters = new List<KeyValuePair<string, string>>()
//                {
//                        new KeyValuePair<string, string>("secret", recaptchaServerToken),
//                        new KeyValuePair<string, string>("response", recaptchaClientResponse)
//                },
//                ContentType = "application/json",

//            });

//        if (response.StatusCode != System.Net.HttpStatusCode.OK)
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }

//        var body = JObject.Parse(response.Content);

//        if (body["success"].Value<bool>() == false)
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }

//        double minimunScore = 0;

//        Double.TryParse(Engine.AppSettings["google-recaptcha-minimun-score"], NumberStyles.Currency, NumberFormatInfo.InvariantInfo, out minimunScore);

//        if (minimunScore > 0)
//        {
//            if (body["score"].Value<double>() < minimunScore)
//            {
//                throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//            }
//        }
//    }

//    private static void ValidateV2(string recaptchaClientResponse)
//    {
//        var recaptchaServerToken = Engine.AppSettings["google-recaptcha-v2-server"];

//        if (string.IsNullOrEmpty(recaptchaServerToken)) return;

//        if (string.IsNullOrEmpty(recaptchaClientResponse))
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }

//        var response = Engine.Resolve<ApiRequestService>().MakeRequest(
//            new RequestParameters()
//            {
//                APIBaseAddress = "https://www.google.com",
//                APIMethodAddress = "/recaptcha/api/siteverify",
//                Method = RestSharp.Method.POST,
//                ParameterType = RestSharp.ParameterType.QueryString,
//                Parameters = new List<KeyValuePair<string, string>>()
//                {
//                        new KeyValuePair<string, string>("secret", recaptchaServerToken),
//                        new KeyValuePair<string, string>("response", recaptchaClientResponse)
//                },
//                ContentType = "application/json",

//            });

//        if (response.StatusCode != System.Net.HttpStatusCode.OK)
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }

//        var body = JObject.Parse(response.Content);

//        if (body["success"].Value<bool>() == false)
//        {
//            throw new RecaptchaException(Engine.Get("InvalidRecaptchaValidation"));
//        }
//    }
//}
