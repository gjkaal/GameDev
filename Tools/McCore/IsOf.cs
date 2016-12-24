using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mc.Core
{
    /// <summary>
    /// Class IsOf contains validator methods for strings
    /// </summary>
    /// <remarks>no remarks</remarks>
    public static class IsOf
    {
        /// <summary>
        /// Check if the specified value is a representation for a number
        /// </summary>
        /// <param name="value">The value under test</param>
        /// <returns><c>true</c> if the string represents a number, <c>false</c> otherwise.</returns>
        /// <remarks>no remarks</remarks>
        public static bool Numeric(string value)
        {
            const string regular = @"^[\-\+]?[0-9\.\,]*$";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the specified value is a representation for an ANSI date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>no comments</remarks>
        public static bool AnsiDate(string value)
        {
            const string regular = @"^[0-9]{8}[T]?[0-9]{0,6}$";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the value provided does not contain characters that are not valid in a filename.        
        /// </summary>
        /// <param name="value">The value under test</param>
        /// <returns><c>true</c> if the string does not contain file name incompatibilities, <c>false</c> otherwise.</returns>
        /// <remarks>Text delimiters are also considered not compatible with a filename ( ' and " )</remarks>
        public static bool ValidFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            char[] whiteList = { ';', ':', '/', '\\', '\'', '"', '?' };
            return value.IndexOfAny(whiteList) <= 0;
        }

        /// <summary>
        /// Check if the value provided does not contain characters that are not valid in a parameter name.        
        /// </summary>
        /// <param name="value">The value under test</param>
        public static bool ParameterName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            char[] whiteList = { ';', ':', '/', '\\', '\'', '"', '?' };
            if (value.IndexOfAny(whiteList) > 0) return false;
            const string regular = @"^[a-zA-Z]{1}[a-zA-Z0-9]*$";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the specified value is a valid email string
        /// </summary>
        /// <param name="value">The value under test</param>
        /// <returns><c>true</c> if the string could be a valid e-mail address, <c>false</c> otherwise.</returns>
        public static bool Email(string value)
        {
            const string regular = @"^[-\w\.]{1,}\@([\d-a-zA-Z]{1,}\.){1,}[\d-a-zA-Z]{2,4}$";
            return TestExpression(regular, value);
        }

        /// <summary>
        ///  Check if the specified value is a valid GUID representation
        /// </summary>
        /// <param name="value">The value under test.</param>
        /// <returns></returns>
        public static bool @Guid(string value)
        {
            const string regular = @"(^[{]?[A-Fa-f0-9]{8}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{12}[}]?$)|"
                + @"(^([0][x])?[A-Fa-f0-9]{8}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{4}[-]?[A-Fa-f0-9]{12}$)|"
                + @"(^([0][x])?[A-Fa-f0-9]{32}$)";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the provided value is an unsigned positive integer value
        /// </summary>
        /// <param name="value">The value under test.</param>
        /// <returns><c>true</c> if the string represents an unsigned positive integer value, <c>false</c> otherwise..</returns>
        public static bool UnsignedInt(string value)
        {
            const string regular = @"^[0-9]*$";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the value contains a valid hexadecimal string
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>no comments</remarks>
        public static bool HexString(string value)
        {
            // length should be dividable by 2          
            if (string.IsNullOrEmpty(value)) return false;
            const string regular = @"^[A-Fa-f0-9]*$";
            return (TestExpression(regular, value) && ((value.Length % 2) == 0));
        }

        /// <summary>
        /// Base64s the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>no comments</remarks>
        public static bool Base64String(string value)
        {
            const string regular = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";
            return TestExpression(regular, value);
        }

        /// <summary>
        /// Check if the provided value is a valid 9 or 8-digit BSN (Burger Service Nummer)
        /// </summary>
        /// <param name="value">The value under test.</param>
        /// <returns><c>true</c> if the string is a valid 8 or 9 digit BSN.</returns>
        public static bool Bsn(string value)
        {
            const int lengteBsn = 9;
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.Length != lengteBsn && value.Length != lengteBsn - 1) return false;
            if (!UnsignedInt(value)) return false;
            if (value.Length == lengteBsn - 1) value = '0' + value;

            var digits = Enumerable.Range(0, lengteBsn)
                .Select(t => Int32.Parse(value.Substring(t, 1), CultureInfo.InvariantCulture) * (lengteBsn - t));
            var enumerable = digits as int[] ?? digits.ToArray();
            var controleGetal = enumerable.Take(lengteBsn - 1).Sum() - enumerable.Last();

            return controleGetal % 11 == 0;
        }

        private static bool TestExpression(string reg, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var regexp = new Regex(reg, RegexOptions.IgnoreCase);
            return regexp.Match(value).Success;
        }
    }
}
