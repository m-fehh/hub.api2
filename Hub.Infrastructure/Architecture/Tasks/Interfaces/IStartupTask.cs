namespace Hub.Infrastructure.Architecture.Tasks.Interfaces
{
    public interface IStartupTask
    {
        void Execute();

        int Order { get; }
    }
}
