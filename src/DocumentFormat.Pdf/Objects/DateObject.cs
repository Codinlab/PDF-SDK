using System;

namespace DocumentFormat.Pdf.Objects
{
    /// <summary>
    /// Represents a Pdf Date Object.
    /// </summary>
    public class DateObject : StringObject
    {
        private DateTimeOffset value;

        /// <summary>
        /// Instanciates a new DateObject from its string representation.
        /// </summary>
        /// <param name="value">The date string.</param>
        public DateObject(string value) : base(value)
        {
            this.value = Parse(value);
        }

        /// <summary>
        /// Instanciates a new DateObject from its value.
        /// </summary>
        /// <param name="value">The date value.</param>
        public DateObject(DateTimeOffset value) : base(Format(value))
        {
            this.value = value;
        }

        /// <summary>
        /// Instanciates a new DateObject from its value.
        /// </summary>
        /// <param name="value">The date value.</param>
        public DateObject(DateTime value) : this(new DateTimeOffset(value))
        {
        }

        /// <summary>
        /// Gets the object's value.
        /// </summary>
        public DateTimeOffset Value => value;

        private static string Format(DateTimeOffset value)
        {
            string offset;
            if(value.Offset == TimeSpan.Zero)
            {
                offset = "Z";
            }
            else
            {
                offset = (value.Offset < TimeSpan.Zero ? "-" : "+") + value.Offset.ToString("hh\\'mm\\'");
            }

            return string.Format("D:{0:yyyyMMddHHmmss}{1}", value, offset);
        }

        private static DateTimeOffset Parse(string value)
        {
            // The prefix "D:", although also optional, is strongly recommended.
            var dateString = value.StartsWith("D:") ? value.Substring(2) : value;
            int year = 0, month = 1, day = 1, hour = 0, minute = 0, second = 0, hh = 0, mm = 0;
            TimeSpan offset;

            // All fields after the year are optional.
            if (dateString.Length < 4)
                throw new FormatException("Invalid date string.");

            year = int.Parse(dateString.Substring(0, 4));

            if(dateString.Length >= 6)
            {
                month = int.Parse(dateString.Substring(4, 2));

                if (dateString.Length >= 8)
                {
                    day = int.Parse(dateString.Substring(6, 2));

                    if (dateString.Length >= 10)
                    {
                        hour = int.Parse(dateString.Substring(8, 2));

                        if (dateString.Length >= 12)
                        {
                            minute = int.Parse(dateString.Substring(10, 2));

                            if (dateString.Length >= 14)
                            {
                                second = int.Parse(dateString.Substring(12, 2));

                            }
                        }
                    }
                }
            }

            DateTime date = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

            if (dateString.Length >= 15)
            {
                // Offset
                if (dateString[14] == 'Z')
                {
                    // UTC
                    offset = TimeSpan.Zero;
                }
                else if (dateString.Length >= 18 && dateString[17] == '\'' && (dateString[14] == '+' || dateString[14] == '-'))
                {
                    hh = int.Parse(dateString.Substring(15, 2));

                    if (dateString.Length >= 21 && dateString[20] == '\'')
                    {
                        mm = int.Parse(dateString.Substring(18, 2));
                    }
                    else
                    {
                        mm = 0;
                    }

                    offset = new TimeSpan(hh, mm, 0);

                    if(dateString[14] == '-')
                    {
                        offset = offset.Negate();
                    }
                }
            }
            else
            {
                // No offset, assume local
                offset = TimeZoneInfo.Local.GetUtcOffset(date);
            }


            return new DateTimeOffset(date, offset);
        }
    }
}
