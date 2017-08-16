using DocumentFormat.Pdf.Extensions;
using DocumentFormat.Pdf.IO;
using System;

namespace DocumentFormat.Pdf
{
    /// <summary>
    /// Defines a <see cref="PdfDocument"/> version
    /// </summary>
    public struct PdfVersion : IComparable, IComparable<PdfVersion>, IEquatable<PdfVersion>
    {
        private int major;
        private int minor;

        private const char separator = '.';
        private const int majorMin = 1;
        private const int minorMin = 0;

        /// <summary>
        /// Gets or sets the Major version
        /// </summary>
        public int Major {
            get { return major; }
            set {
                if (value < majorMin)
                    throw new ArgumentOutOfRangeException();
                major = value;
            }
        }

        /// <summary>
        /// Gets or sets the Minor version
        /// </summary>
        public int Minor {
            get { return minor; }
            set {
                if (value < minorMin)
                    throw new ArgumentOutOfRangeException();
                minor = value;
            }
        }

        /// <summary>
        /// Instanciates PdfVersion from major and minor numbers
        /// </summary>
        /// <param name="major">Major number</param>
        /// <param name="minor">Minor number</param>
        public PdfVersion(int major, int minor)
        {
            if (major < majorMin)
                throw new ArgumentOutOfRangeException(nameof(major));

            if (minor < minorMin)
                throw new ArgumentOutOfRangeException(nameof(minor));

            this.major = major;
            this.minor = minor;
        }

        /// <summary>
        /// Instanciates PdfVersion from version string (i.e.: "1.5")
        /// </summary>
        /// <param name="version">Version string</param>
        public PdfVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            var versionArray = version.Split(separator);

            if (versionArray.Length < 2)
                throw new ArgumentException("Version should specify a major and a minor version number.", nameof(version));

            if (!int.TryParse(versionArray[0], out major))
                throw new ArgumentException("Major version should be an integer.", nameof(version));
            else if (major < majorMin)
                throw new ArgumentException($"Major version should be greater than {majorMin}.", nameof(version));

            if (!int.TryParse(versionArray[1], out minor))
                throw new ArgumentException("Minor version should be an integer.", nameof(version));
            else if (minor < minorMin)
                throw new ArgumentException($"Minor version should be greater than {minorMin}.", nameof(version));
        }

        /// <summary>
        /// Writes PdfVersion.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public void Write(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(ToString());
        }

        /// <summary>
        /// Writes PDF header with current PdfVersion.
        /// </summary>
        /// <param name="writer">The <see cref="PdfWriter"/> to use.</param>
        public void WriteHeader(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteLine(PdfDocument.PdfHeader + ToString());
        }

        public int CompareTo(PdfVersion other)
        {
            if (other.Major != major)
                if (other.Major > major)
                    return -1;
                else
                    return 1;

            if (other.Minor != minor)
                if (other.Minor > minor)
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

        public override bool Equals(object obj)
        {
            return obj is PdfVersion && this == (PdfVersion)obj;
        }

        public override int GetHashCode()
        {
            return major.GetHashCode() ^ minor.GetHashCode();
        }

        public bool Equals(PdfVersion other)
        {
            return other.Major == major && other.Minor == minor;
        }

        /// <summary>
        /// ToString method override
        /// </summary>
        /// <returns>String representation of current instance.</returns>
        public override string ToString()
        {
            return string.Join(separator.ToString(), major, minor);
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="a">Left value</param>
        /// <param name="b">Right value</param>
        /// <returns>True if a equals b, otherwise false.</returns>
        public static bool operator ==(PdfVersion a, PdfVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor;
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        /// <param name="a">Left value</param>
        /// <param name="b">Right value</param>
        /// <returns>False if a equals b, otherwise true.</returns>
        public static bool operator !=(PdfVersion a, PdfVersion b)
        {
            return a.Major != b.Major || a.Minor != b.Minor;
        }

        /// <summary>
        /// Reads Pdf file's header and return Pdf Version.
        /// </summary>
        /// <param name="reader">The <see cref="PdfReader"/> to use.</param>
        /// <returns>Pdf Version of the file.</returns>
        public static PdfVersion FromReader(PdfReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.Position = 0;

            var header = reader.ReadLine();
            if (header == null || !header.StartsWith(PdfDocument.PdfHeader))
                throw new FormatException("Invalid file header");

            return new PdfVersion(header.Substring(PdfDocument.PdfHeader.Length));
        }
    }
}
