namespace Pictua.Core
{
    using System;

    public interface IDiff
    {
        IClientIdentity Author { get; }

        DateTime Time { get; }
        IError? Error { get; }
    }
}
