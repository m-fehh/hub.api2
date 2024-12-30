using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.HealthChecker.Checkers
{
    public static class DbConnectionChecker
    {
        public static bool CheckSqlServer(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return false;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
