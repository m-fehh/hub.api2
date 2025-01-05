namespace Hub.Infrastructure.Architecture.HealthChecker.Checkers
{
    public static class UrlChecker
    {
        private static HttpClient _httpClient = new HttpClient();

        public static bool Check(string url, bool acceptUnauthorizedAsValid)
        {
            if (string.IsNullOrEmpty(url)) return false;

            try
            {
                using (var response = _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).GetAwaiter().GetResult())
                {
                    return response.StatusCode == System.Net.HttpStatusCode.OK || acceptUnauthorizedAsValid && response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
                }
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
