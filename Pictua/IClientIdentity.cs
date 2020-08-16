using System;

namespace Pictua
{
    public struct ClientIdentity
    {
        public string Name { get; }
        public Guid Guid { get; }

        public ClientIdentity(string name, Guid? guid = null)
        {
            Name = name;
            Guid = guid ?? Guid.NewGuid();
        }

        public override bool Equals(object? obj) => obj is ClientIdentity identity && Guid == identity.Guid;
        public override int GetHashCode() => HashCode.Combine(Guid);
    }
}
