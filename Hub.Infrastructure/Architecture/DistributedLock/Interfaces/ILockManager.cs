namespace Hub.Infrastructure.Architecture.DistributedLock.Interfaces
{
    public interface ILockManager : IDisposable
    {
        void Init();
        IDisposable Lock(string resource, double expiryTimeInSeconds = 30);
    }
}
