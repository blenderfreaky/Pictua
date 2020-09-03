using System;

namespace Pictua
{
    public struct ClientIdentity : IEquatable<ClientIdentity>
    {
        public string Name { get; }
        public Guid Guid { get; }

        public ClientIdentity(string name, Guid? guid = null)
        {
            Name = name;
            Guid = guid ?? Guid.NewGuid();
        }

        public override bool Equals(object? obj) => obj is ClientIdentity identity && Equals(identity);

        public bool Equals(ClientIdentity other) => Guid == other.Guid;

        public override int GetHashCode() => HashCode.Combine(Guid);

        public static bool operator ==(ClientIdentity left, ClientIdentity right) => left.Equals(right);

        public static bool operator !=(ClientIdentity left, ClientIdentity right) => !(left == right);
    }
}