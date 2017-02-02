using DocumentFormat.Pdf.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormat.Pdf.Internal.Objects
{
    public class NumericObject : PdfObject
    {
        private readonly long _longValue;
        private readonly double _doubleValue;
        private readonly bool _isReal;

        public bool IsInteger {
            get { return !_isReal; }
        }

        public bool IsReal {
            get { return _isReal; }
        }

        public int Int32Value {
            get {
                return _isReal ? Convert.ToInt32(_doubleValue) : Convert.ToInt32(_longValue);
            }
        }

        public long Int64Value {
            get {
                return _isReal ? Convert.ToInt64(_doubleValue) : _longValue;
            }
        }

        public float SingleValue {
            get {
                return _isReal ? Convert.ToSingle(_doubleValue) : Convert.ToSingle(_longValue);
            }
        }

        public double DoubleValue {
            get {
                return _isReal ? _doubleValue : Convert.ToDouble(_longValue);
            }
        }

        public NumericObject(string content)
        {
            _isReal = content.IndexOf('.') != -1;
            if (_isReal)
            {
                _doubleValue = double.Parse(content);
            }
            else
            {
                _longValue = long.Parse(content);
            }
        }

        public static async Task<NumericObject> CreateFromReaderAsync(DocumentReader reader, char? firstChar = null)
        {
            var sb = new StringBuilder();
            if (firstChar.HasValue)
            {
                sb.Append(firstChar.Value);
            }
            sb.Append(await reader.ReadCharsToWhiteSpaceAsync());
            return new NumericObject(sb.ToString());
        }
    }
}
