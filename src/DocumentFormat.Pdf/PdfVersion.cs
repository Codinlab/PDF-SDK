using System;

namespace DocumentFormat.Pdf
{
    /// <summary>
    /// Defines a <see cref="PdfDocument"/> version
    /// </summary>
    public struct PdfVersion : IComparable, IComparable<PdfVersion>, IEquatable<PdfVersion>
    {
        private int _major;
        private int _minor;

        private const char separator = '.';
        private const int majorMin = 1;
        private const int minorMin = 0;

        /// <summary>
        /// Gets or sets the Major version
        /// </summary>
        public int Major {
            get { return _major; }
            set {
                if (value < majorMin)
                    throw new ArgumentOutOfRangeException();
                _major = value;
            }
        }

        /// <summary>
        /// Gets or sets the Minor version
        /// </summary>
        public int Minor {
            get { return _minor; }
            set {
                if (value < minorMin)
                    throw new ArgumentOutOfRangeException();
                _minor = value;
            }
        }

        public PdfVersion(int major, int minor)
        {
            if (major < majorMin)
                throw new ArgumentOutOfRangeException(nameof(major));

            if (minor < minorMin)
                throw new ArgumentOutOfRangeException(nameof(minor));

            _major = major;
            _minor = minor;
        }

        public PdfVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            var versionArray = version.Split(separator);

            if (versionArray.Length < 2)
                throw new ArgumentException("Version should specify a major and a minor version number.", nameof(version));

            if (!int.TryParse(versionArray[0], out _major))
                throw new ArgumentException("Major version should be an integer.", nameof(version));
            else if (_major < majorMin)
                throw new ArgumentException($"Major version should be greater than {majorMin}.", nameof(version));

            if (!int.TryParse(versionArray[1], out _minor))
                throw new ArgumentException("Minor version should be an integer.", nameof(version));
            else if (_minor < minorMin)
                throw new ArgumentException($"Minor version should be greater than {minorMin}.", nameof(version));
        }

        public int CompareTo(PdfVersion other)
        {
            if (other.Major != _major)
                if (other.Major > _major)
                    return -1;
                else
                    return 1;

            if (other.Minor != _minor)
                if (other.Minor > _minor)
                    return -1;
                else
                    return 1;

            return 0;
        }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if(obj is PdfVersion)
            {
                return CompareTo((PdfVersion)obj);
            }
            else
            {
                throw new ArgumentException($"{nameof(obj)} parameter shoud be a {nameof(PdfVersion)}.", nameof(obj));
            }
        }

        public bool Equals(PdfVersion other)
        {
            return other.Major == _major && other.Minor == _minor;
        }

        public override string ToString()
        {
            return string.Concat(_major, separator, _minor);
        }
    }
}
