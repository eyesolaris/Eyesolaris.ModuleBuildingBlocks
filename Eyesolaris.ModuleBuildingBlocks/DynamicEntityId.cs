using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    /// <summary>
    /// A class representing the entity identifier consisting of an entity name and version
    /// </summary>
    public readonly struct DynamicEntityId : IEquatable<DynamicEntityId>, IComparable<DynamicEntityId>, IComparable
    {
        public readonly string Name => _innerValue.Name!;

        public readonly Version Version => _innerValue.Version!;

        public readonly bool IsEmpty => _innerValue.IsEmpty;

        public DynamicEntityId(string name, Version version)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(version, nameof(version));
            _innerValue = new(name, version);
        }

        private DynamicEntityId(DynamicEntityIdTemplate template)
        {
            _innerValue = template;
        }

        public string ToParsableString()
            => _innerValue.ToParsableString();

        public static bool TryParse(ReadOnlySpan<char> text, out DynamicEntityId value)
        {
            value = default;
            if (!DynamicEntityIdTemplate.TryParse(text, out DynamicEntityIdTemplate template))
            {
                return false;
            }
            if (template.Name is null || template.Version is null)
            {
                return false;
            }
            value = new(template);
            return true;
        }

        public static DynamicEntityId Parse(ReadOnlySpan<char> text)
        {
            DynamicEntityIdTemplate template = DynamicEntityIdTemplate.Parse(text);
            if (template.Name is null)
            {
                throw new FormatException("The input string doesn't contain an entity name component");
            }
            if (template.Version is null)
            {
                throw new FormatException("The input string doesn't contain an entity version component");
            }
            return new(template);
        }

        public override int GetHashCode() => _innerValue.GetHashCode();

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is DynamicEntityId otherName)
            {
                return Equals(otherName);
            }
            return false;
        }

        public bool Equals(DynamicEntityId other)
            => _innerValue.Equals(other._innerValue);

        public int CompareTo(DynamicEntityId other)
            => _innerValue.CompareTo(other._innerValue);

        public int CompareTo(object? obj)
        {
            if (obj is not DynamicEntityId other)
            {
                throw new ArgumentException($"Object is not of type {typeof(DynamicEntityId).FullName}", nameof(obj));
            }
            return CompareTo(other);
        }

        public static bool operator ==(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue == right._innerValue;

        public static bool operator !=(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue != right._innerValue;

        public static bool operator <(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue < right._innerValue;

        public static bool operator <=(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue <= right._innerValue;

        public static bool operator >(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue > right._innerValue;

        public static bool operator >=(DynamicEntityId left, DynamicEntityId right)
            => left._innerValue >= right._innerValue;

        private readonly DynamicEntityIdTemplate _innerValue;
    }
}
