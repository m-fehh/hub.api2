using StackExchange.Redis;

namespace Hub.Infrastructure.Architecture.HealthChecker.Checkers
{
    public static class RedisChecker
    {
        public static bool CheckConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return false;

            try
            {
                using (var conn = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString)))
                {
                    return conn.IsConnected;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
