namespace Hub.Infrastructure.Autofac.Interfaces
{
    public interface ISingleBase<T>
    {
        T Value { get; }

        void SetValue(T value);
    }
}
