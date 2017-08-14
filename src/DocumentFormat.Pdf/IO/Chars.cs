namespace DocumentFormat.Pdf.IO
{
    public static class Chars
    {
        /// <summary>
        /// Null character
        /// </summary>
        public const char NUL = '\x00';
        /// <summary>
        /// Back space character
        /// </summary>
        public const char BS = '\x08';
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

        /// <summary>
        /// Tests if character is considered as white space
        /// </summary>
        /// <param name="c">Tested character</param>
        /// <returns>True if character is considered as whitespace, otherwise false</returns>
        public static bool IsWhiteSpace(char c)
        {
            return c == NUL || c == HT || c == LF || c == FF || c == CR || c == SP;
        }

        /// <summary>
        /// Tests if character is considered as an end of line marker
        /// </summary>
        /// <param name="c">Tested character</param>
        /// <returns>True if character is considered as an end of line marker, otherwise false</returns>
        public static bool IsEndOfLine(char c)
        {
            return c == LF || c == CR;
        }

        /// <summary>
        /// Tests if character is considered as a delimiter
        /// </summary>
        /// <param name="c">Tested character</param>
        /// <returns>True if character is considered as a delimiter, otherwise false</returns>
        public static bool IsDelimiter(char c)
        {
            return c == '(' || c == ')' || c == '<' || c == '>' || c == '[' || c == ']' || c == '{' || c == '}' || c == '/' || c == '%';
        }

        /// <summary>
        /// Tests if character is considered as a delimiter or white space
        /// </summary>
        /// <param name="c">Tested character</param>
        /// <returns>True if character is considered as a delimiter or white space, otherwise false</returns>
        public static bool IsDelimiterOrWhiteSpace(char c)
        {
            return IsDelimiter(c) || IsWhiteSpace(c);
        }
    }
}
