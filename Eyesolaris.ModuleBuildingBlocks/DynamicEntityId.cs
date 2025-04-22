using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    /// <summary>
    /// A class representing the entity identifier consisting of an entity name and version
    /// </summary>
    public readonly struct DynamicEntityId : IEquatable<DynamicEntityId>, IComparable<DynamicEntityId>, IComparable
    {
        /// <summary>
        /// The exact entity identifier
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="DynamicEntityIdTemplate"/> value is empty</exception>
        public readonly string Name => _innerValue.Name!;

        /// <summary>
        /// The exact entity version
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="DynamicEntityIdTemplate"/> value is empty</exception>
        public readonly Version Version => _innerValue.Version!;

        /// <summary>
        /// Indicates whether the value was properly constructed
        /// </summary>
        public readonly bool IsEmpty => _innerValue.IsEmpty;

        /// <summary>
        /// Constructs a dynamic entity ID
        /// </summary>
        /// <param name="name">An entity name</param>
        /// <param name="version">An entity version</param>
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

        /// <summary>
        /// Creates a machine-readable object representation
        /// </summary>
        /// <returns>A machine-readable object representation as a string</returns>
        public string ToParsableString()
            => _innerValue.ToParsableString();

        /// <summary>
        /// Converts id object to id template object. Always successful
        /// </summary>
        /// <returns>A converted object</returns>
        public DynamicEntityIdTemplate ToTemplate()
            => _innerValue;

        /// <summary>
        /// Returns the human-readable object representation
        /// </summary>
        /// <returns>String containing the object representation</returns>
        public override string ToString()
            => _innerValue.ToString();

        /// <summary>
        /// Tries to parse a machine-readable ID representation
        /// </summary>
        /// <param name="text">An object representation</param>
        /// <param name="value">A parsed value</param>
        /// <returns>A result of the operation</returns>
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

        /// <summary>
        /// Parses a machine-readable ID representation. Throws if unsuccessful
        /// </summary>
        /// <param name="text">An object representation</param>
        /// <returns>A parsed object</returns>
        /// <exception cref="FormatException">A text has an invalid or unsupported format</exception>
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
