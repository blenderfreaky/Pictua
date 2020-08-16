namespace Pictua
{
    using System;

    public interface IChange
    {
        IClientIdentity Author { get; }

        DateTime Time { get; }

        IChange WithTime(DateTime dateTime);
    }
}
