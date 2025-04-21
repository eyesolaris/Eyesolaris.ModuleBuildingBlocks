using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eyesolaris.DynamicLoading
{
    public static class VersionUtilities
    {
        public static EqualityComparer<Version> EqualityComparer
            => VersionEqualityComparer.Instance;

        public static Comparer<Version> Comparer
            => VersionComparer.Instance;

        public static Version GetTypeAssemblyVersion(this Type type, bool normalize = true)
        {
            AssemblyName typeAssemblyName = type.Assembly.GetName();
            Version ver = typeAssemblyName.Version ?? DefaultVersionValue;
            if (normalize)
            {
                return ver.Normalize();
            }
            return ver;
        }

        public static Version GetTypeAssemblyVersion<T>(T obj, bool normalize = true)
            where T : notnull
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return GetTypeAssemblyVersion(obj.GetType(), normalize);
        }

        public static string ToNormalizedString(this Version version)
        {
            int fieldCount = 4;
            if (version.Revision == 0 && version.Build == 0)
            {
                fieldCount = 2;
            }
            else if (version.Revision == 0)
            {
                fieldCount = 3;
            }
            return version.ToString(fieldCount);
        }

        public static Version ParseAsNormalized(string versionString)
        {
            Version temp = Version.Parse(versionString);
            if (temp.Build == 0 && temp.Revision == 0)
            {
                return new Version(temp.Major, temp.Minor);
            }
            else if (temp.Revision == 0)
            {
                return new Version(temp.Major, temp.Minor, temp.Build);
            }
            return temp;
        }

        public static Version Normalize(this Version version)
        {
            if (version.Build == 0 && version.Revision == 0)
            {
                return new Version(version.Major, version.Minor);
            }

            else if (version.Revision == 0)
            {
                return new Version(version.Major, version.Minor, version.Build);
            }
            return version;
        }

        public static string NormalizeString(string versionString)
        {
            Version tmpVersion = Version.Parse(versionString);
            tmpVersion = tmpVersion.Normalize();
            return tmpVersion.ToString();
        }

        public static bool CompareEqualNormalized(Version? a, Version? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            bool aRevisionZero = a.Revision <= 0;
            bool bRevisionZero = b.Revision <= 0;
            bool aBuildZero = a.Build <= 0;
            bool bBuildZero = b.Build <= 0;
            return a.Major == b.Major
                && a.Minor == b.Minor
                && (aBuildZero && bBuildZero || (a.Build == b.Build))
                && (aRevisionZero && bRevisionZero || (a.Revision == b.Revision));
        }

        public static int GetHashCodeNormalized(Version obj)
        {
            // Based on .NET 6 standard Version.GetHashCode()

            // Let's assume that most version numbers will be pretty small and just
            // OR some lower order bits together.
            int accumulator = 0;
            accumulator |= (obj.Major & 0x0000000F) << 28;
            accumulator |= (obj.Minor & 0x000000FF) << 20;
            accumulator |= ((obj.Build > 0 ? obj.Build : 0) & 0x000000FF) << 12;
            accumulator |= ((obj.Revision > 0 ? obj.Revision : 0) & 0x00000FFF);
            return accumulator;
        }

        public static int CompareNormalized(Version? a, Version? b)
        {
            if (a is null && b is null)
            {
                return 0;
            }
            if (a is null)
            {
                return -1;
            }
            if (b is null)
            {
                return 1;
            }
            int result = a.Major - b.Major;
            if (result != 0)
            {
                return result;
            }
            result = a.Minor - b.Minor;
            if (result != 0)
            {
                return result;
            }
            int aBuild = a.Build > 0 ? a.Build : 0;
            int bBuild = b.Build > 0 ? b.Build : 0;
            result = aBuild - bBuild;
            if (result != 0)
            {
                return result;
            }
            int aRevision = a.Revision > 0 ? a.Revision : 0;
            int bRevision = b.Revision > 0 ? b.Revision : 0;
            return aRevision - bRevision;
        }

        public static readonly Version DefaultVersionValue = new(1, 0);

        private class VersionEqualityComparer : EqualityComparer<Version>
        {
            public override bool Equals(Version? x, Version? y)
                => CompareEqualNormalized(x, y);

            public override int GetHashCode(Version obj)
                => GetHashCodeNormalized(obj);

            internal static VersionEqualityComparer Instance { get; } = new();
            private VersionEqualityComparer()
            {
            }
        }

        private class VersionComparer : Comparer<Version>
        {
            public override int Compare(Version? x, Version? y)
                => CompareNormalized(x, y);

            internal static VersionComparer Instance { get; } = new();

            private VersionComparer()
            {
            }
        }
    }
}
