namespace Hub.Infrastructure.Autofac
{
    public class TenantLifeTimeScope
    {
        private static readonly AsyncLocal<TenantContextHolder> TenantHolder = new();

        /// <summary>
        /// Gets current tenant context
        /// </summary>
        public static string? CurrentTenant => TenantHolder.Value?.Tenant;

        public static IDisposable Start(string tenant)
        {
            TenantHolder.Value = new TenantContextHolder { Tenant = tenant };
            return new ContextDisposable();
        }

        public static void Dispose()
        {
            var holder = TenantHolder.Value;
            if (holder != null)
            {
                holder.Tenant = null;
            }
        }
    }

    public sealed class ContextDisposable : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose();
                _disposed = true;
            }
        }
    }

    public class TenantContextHolder
    {
        public string? Tenant { get; set; }
    }
}