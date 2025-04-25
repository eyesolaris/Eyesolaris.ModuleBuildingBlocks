using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    /// <summary>
    /// A class representing the entity identifier template for making requests and searches
    /// </summary>
    public readonly struct DynamicEntityIdTemplate : IEquatable<DynamicEntityIdTemplate>, IComparable<DynamicEntityIdTemplate>, IComparable
    {
        /// <summary>
        /// Constructs an entity identifier template with name and version components
        /// </summary>
        /// <remarks>
        /// Please note that a null version value means something specific,
        /// for example, the last version, the default versionm or any version, depending on the context
        /// </remarks>
        /// <param name="name">A concrete entity name</param>
        /// <param name="version">An entity version template. <see langword="null"/> version denotes something special depending on the context</param>
        public DynamicEntityIdTemplate(string name, Version? version)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            _name = name;
            _version = version;
            _isConstructed = true;
        }

        /// <summary>
        /// Constructs an identifier template denoting any entity
        /// </summary>
        public DynamicEntityIdTemplate()
        {
            _name = null;
            _version = null;
            _isConstructed = true;
        }

        /// <summary>
        /// Constructs an entity identifier template with a name component.
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> name means something special depending on the context,
        /// for example, any entity or some default entity
        /// <br/>
        /// If <paramref name="name"/> is <see langword="null"/>, the <see cref="Version"/>
        /// property will be set to <see langword="null"/>, too
        /// </remarks>
        /// <param name="name">An entity name template. <see langword="null"/> name means something special depending on the context,
        /// for example, any entity or some default entity</param>
        public DynamicEntityIdTemplate(string? name)
        {
            _name = name;
            _version = null;
            _isConstructed = true;
        }

        /// <summary>
        /// The requested entity identifier. If <see langword="null"/>, version will be <see langword="null"/>, too.
        /// <see langword="null"/> value denotes any entity name
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="DynamicEntityIdTemplate"/> value is empty</exception>
        public readonly string? Name => IsEmpty ? throw new InvalidOperationException() : _name;

        /// <summary>
        /// The requested entity version. If <see langword="null"/>, denotes something specific
        /// for search functions, for example, the last version, the default version, or any version available
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="DynamicEntityIdTemplate"/> value is empty</exception>
        public readonly Version? Version => IsEmpty ? throw new InvalidOperationException() : _version;

        /// <summary>
        /// Indicates whether the value was properly constructed
        /// </summary>
        public bool IsEmpty => !_isConstructed;

        public override int GetHashCode()
            => HashCode.Combine(Name, Version);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is DynamicEntityIdTemplate otherName)
            {
                return Equals(otherName);
            }
            return false;
        }

        public bool Equals(DynamicEntityIdTemplate other)
            => other.Name == Name && other.Version == Version;

        /// <summary>
        /// Returns the human-readable object representation
        /// </summary>
        /// <returns>String containing the object representation</returns>
        public override string ToString()
        {
            if (Name is null)
            {
                return "(any entity)";
            }
            else if (Version is null)
            {
                return Name;
            }
            return $"{Name} v.{Version}";
        }

        /// <summary>
        /// Creates a machine-readable object representation
        /// </summary>
        /// <returns>A machine-readable object representation as a string</returns>
        public string ToParsableString()
        {
            if (Name is null && Version is null)
            {
                return string.Empty;
            }
            if (Version is null && Name is not null)
            {
                return Name!;
            }
            else
            {
                return $"{Name}{SPLITTER}{Version!.ToNormalizedString()}";
            }
        }

        /// <summary>
        /// Converts a tempalte id object to an id object. Unsuccsessful
        /// when the template has omissions 
        /// </summary>
        /// <returns>A converted object</returns>
        /// <exception cref="InvalidOperationException">A template has omissions</exception>
        public DynamicEntityId ToId()
        {
            if (Name is null || Version is null)
            {
                throw new InvalidOperationException("Template has omissions");
            }
            return new DynamicEntityId(Name, Version);
        }

        /// <summary>
        /// Tries to parse <see cref="DynamicEntityIdTemplate"/>
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="value">Parsed value</param>
        /// <returns><see langword="true"/> if parsing is successful. Otherwise, <see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> text, out DynamicEntityIdTemplate value)
        {
            text = text.Trim();
            if (text.IsEmpty)
            {
                value = new();
                return true;
            }
            int splitterIndex = text.LastIndexOf(SPLITTER);
            if (splitterIndex != -1)
            {
                ReadOnlySpan<char> idRaw = text[..splitterIndex];
                ReadOnlySpan<char> versionRaw = text[(splitterIndex + 1)..];
                if (!VersionUtilities.TryParseAsNormalized(versionRaw, out Version? version))
                {
                    value = default;
                    return false;
                }
                value = new(idRaw.ToString(), version.Normalize());
                return true;
            }
            value = new(text.ToString());
            return true;
        }

        /// <summary>
        /// Parses <see cref="DynamicEntityIdTemplate"/> from text
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Parsed value</returns>
        /// <exception cref="FormatException"/>
        public static DynamicEntityIdTemplate Parse(ReadOnlySpan<char> text)
        {
            text = text.Trim();
            if (text.IsEmpty)
            {
                return new();
            }
            int splitterIndex = text.LastIndexOf(SPLITTER);
            if (splitterIndex != -1)
            {
                ReadOnlySpan<char> idRaw = text[..splitterIndex];
                ReadOnlySpan<char> versionRaw = text[(splitterIndex + 1)..];
                Version version;
                try
                {
                    version = Version.Parse(versionRaw);
                }
                catch (Exception ex)
                {
                    throw new FormatException("The entity ID's version component has an invalid format", ex);
                }
                return new(idRaw.ToString(), version);
            }
            return new(text.ToString());
        }

        public int CompareTo(DynamicEntityIdTemplate other)
        {
            if (Equals(other))
            {
                return 0;
            }
            if (this < other)
            {
                return -1;
            }
            return 1;
        }

        public int CompareTo(object? obj)
        {
            if (obj is not DynamicEntityIdTemplate other)
            {
                throw new ArgumentException($"Object is not of type {typeof(DynamicEntityIdTemplate).FullName}", nameof(obj));
            }
            return CompareTo(other);
        }

        public static bool operator ==(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
            => left.Equals(right);

        public static bool operator !=(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
            => !(left == right);

        public static bool operator <(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
        {
            if (string.CompareOrdinal(left.Name, right.Name) < 0)
            {
                return true;
            }
            if (left.Name == right.Name && left.Version < right.Version)
            {
                return true;
            }
            return false;
        }

        public static bool operator <=(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
        {
            if (left == right)
            {
                return true;
            }
            return left < right;
        }

        public static bool operator >(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
            => right < left;

        public static bool operator >=(DynamicEntityIdTemplate left, DynamicEntityIdTemplate right)
            => right <= left;


        private const char SPLITTER = '@';

        private readonly string? _name;
        private readonly Version? _version;

        private readonly bool _isConstructed;
    }
}
