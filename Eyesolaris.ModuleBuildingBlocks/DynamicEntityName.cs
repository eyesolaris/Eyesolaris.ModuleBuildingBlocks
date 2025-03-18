using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    public readonly struct DynamicEntityName : IEquatable<DynamicEntityName>
    {
        public DynamicEntityName(string name, Version version)
        {
            Id = name;
            Version = version;
        }

        public readonly string Id { get; }
        public readonly Version Version { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Version);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is DynamicEntityName otherName)
            {
                return Equals(otherName);
            }
            return false;
        }

        public bool Equals(DynamicEntityName other)
        {
            return other.Id == Id && other.Version == Version;
        }

        public override string ToString()
        {
            return $"{Id} {Version}";
        }

        public static bool operator ==(DynamicEntityName left, DynamicEntityName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DynamicEntityName left, DynamicEntityName right)
        {
            return !(left == right);
        }

        public static bool operator <(DynamicEntityName left, DynamicEntityName right)
        {
            if (left.Id == right.Id && left.Version < right.Version)
            {
                return true;
            }
            return false;
        }

        public static bool operator >(DynamicEntityName left, DynamicEntityName right)
        {
            if (left.Id == right.Id && left.Version > right.Version)
            {
                return true;
            }
            return false;
        }
    }
}
