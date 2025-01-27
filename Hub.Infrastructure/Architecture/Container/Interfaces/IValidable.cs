﻿namespace Hub.Infrastructure.Architecture.Autofac.Interfaces
{
    public interface IValidable<T>
    {
        void Validate(T value);
    }

    public interface IValidable
    {
        void Validate();
    }
}
