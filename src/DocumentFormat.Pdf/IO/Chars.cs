namespace DocumentFormat.Pdf.IO
{
    public static class Chars
    {
        /// <summary>
        /// Null character
        /// </summary>
        public const char NUL = '\x00';
        /// <summary>
        /// Tab character
        /// </summary>
        public const char HT = '\x09';
        /// <summary>
        /// Line feed character
        /// </summary>
        public const char LF = '\x0A';
        /// <summary>
        /// Form Feed character
        /// </summary>
        public const char FF = '\x0C';
        /// <summary>
        /// Carriage return character
        /// </summary>
        public const char CR = '\x0D';
        /// <summary>
        /// Space character
        /// </summary>
        public const char SP = '\x20';

        public static bool IsWhiteSpace(char c)
        {
            return c == NUL || c == HT || c == LF || c == FF || c == CR || c == SP;
        }
    }
}
