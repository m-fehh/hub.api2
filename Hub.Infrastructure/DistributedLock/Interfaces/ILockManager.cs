namespace Hub.Infrastructure.DistributedLock.Interfaces
{
    public interface ILockManager : IDisposable
    {
        void Init();
        IDisposable Lock(string resource, double expiryTimeInSeconds = 30);
    }
}
