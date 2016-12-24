using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mc.Core
{
    /// <summary>
    /// Class ConvertTo contains generic conversion routines
    /// </summary>
    /// <remarks>no remarks</remarks>
    public static class ConvertTo
    {

        /// <summary>
        /// Splits a string in key-value pairs.
        /// If no aSeparator is specified, \r\n is used
        /// A key-value pair is separated by '='
        /// When type conversions are needed, the InvariantCulture is used
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="values">a values.</param>
        /// <param name="separator">a separator.</param>
        /// <returns>a Dictionary&lt;TKey, TValue&gt;</returns>
        /// <remarks>no remarks</remarks>
        public static IDictionary<TKey, TValue> Dictionary<TKey, TValue>(this string values, params char[] separator)
        {
            // empty resultset if no data was presented
            if (string.IsNullOrEmpty(values)) return new Dictionary<TKey, TValue>();
            // use a default seperator
            if (separator == null || separator.Length == 0) separator = new char[] { '\r', '\n' };
            var sValuesArray = values.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            //
            var r = new Dictionary<TKey, TValue>();
            foreach (var s in sValuesArray)
            {
                var keySeparator = s.IndexOf('=');
                var key = (TKey)Convert.ChangeType(s.Substring(0, keySeparator), typeof(TKey), CultureInfo.InvariantCulture);
                var val = (TValue)Convert.ChangeType(s.Substring(keySeparator + 1), typeof(TValue), CultureInfo.InvariantCulture);
                r.Add(key, val);
            }
            return r;
        }

        /// <summary>
        /// convert a base32 string to its byte array equivalent
        /// </summary>
        /// <param name="base32EncodedUnicode">The base32 encoded uni code.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.FormatException">The provided string does not appear to be Base32 encoded: + Environment.NewLine + base32EncodedUnicode + Environment.NewLine</exception>
        /// <remarks>no remarks</remarks>
        public static byte[] Base32ToBytes(string base32EncodedUnicode)
        {
            if (string.IsNullOrWhiteSpace(base32EncodedUnicode))
            {
                return null;
            }
            try
            {
                return FromBase32String(base32EncodedUnicode);
            }
            catch (System.FormatException ex)
            {
                throw new System.FormatException("The provided string does not appear to be Base32 encoded:" + Environment.NewLine + base32EncodedUnicode + Environment.NewLine, ex);
            }
        }

        /// <summary>
        /// Convert th string value to an array of value, using base32 decoding
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>System.Byte[].</returns>
        /// <remarks>no remarks</remarks>
        public static byte[] FromBase32String(string value)
        {
            if (string.IsNullOrEmpty(value)) return new byte[0];
            var numBytes = value.Length * 5 / 8;
            var bytes = new byte[numBytes];

            // all UPPERCASE chars
            value = value.ToUpper();

            int bitBuffer;
            int currentCharIndex;
            int bitsInBuffer;

            if (value.Length < 3)
            {
                bytes[0] = (byte)(ValidBase32Chars.IndexOf(value[0]) | ValidBase32Chars.IndexOf(value[1]) << 5);
                return bytes;
            }

            bitBuffer = (ValidBase32Chars.IndexOf(value[0]) | ValidBase32Chars.IndexOf(value[1]) << 5);
            bitsInBuffer = 10;
            currentCharIndex = 2;
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)bitBuffer;
                bitBuffer >>= 8;
                bitsInBuffer -= 8;
                while (bitsInBuffer < 8 && currentCharIndex < value.Length)
                {
                    bitBuffer |= ValidBase32Chars.IndexOf(value[currentCharIndex++]) << bitsInBuffer;
                    bitsInBuffer += 5;
                }
            }

            return bytes;
        }

        /// <summary>
        /// The valid base32 chars
        /// </summary>
        private static string ValidBase32Chars = "QAZ2WSX3" + "EDC4RFV5" + "TGB6YHN7" + "UJM8K9LP";


        /// <summary>
        /// Converts an array of value to a Base32-k string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        /// <remarks>Code from : http://www.atrevido.net/blog/PermaLink.aspx?guid=debdd47c-9d15-4a2f-a796-99b0449aa8af</remarks>
        public static string Base32(byte[] value)
        {
            if (value == null || value.Length == 0) return string.Empty;
            var sb = new StringBuilder();         // holds the base32 chars
            var hi = 5;
            var currentByte = 0;

            while (currentByte < value.Length)
            {
                // do we need to use the next byte?
                byte index;
                if (hi > 8)
                {
                    // get the last piece from the current byte, shift it to the right
                    // and increment the byte counter
                    index = (byte)(value[currentByte++] >> (hi - 5));
                    if (currentByte != value.Length)
                    {
                        // if we are not at the end, get the first piece from
                        // the next byte, clear it and shift it to the left
                        index = (byte)(((byte)(value[currentByte] << (16 - hi)) >> 3) | index);
                    }

                    hi -= 3;
                }
                else if (hi == 8)
                {
                    index = (byte)(value[currentByte++] >> 3);
                    hi -= 3;
                }
                else
                {

                    // simply get the stuff from the current byte
                    index = (byte)((byte)(value[currentByte] << (8 - hi)) >> 3);
                    hi += 5;
                }

                sb.Append(ValidBase32Chars[index]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert the universalDateTime string to a universal date time object.
        /// </summary>
        /// <param name="universalDateTime">The univeral date time.</param>
        /// <returns>DateTime.</returns>
        /// <remarks>no remarks</remarks>
        public static DateTime FromUniversalDateTimeString(string universalDateTime)
        {
            return DateTime.ParseExact(universalDateTime, UniversalDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime();
        }

        /// <summary>
        /// Convert the dateTime object to a universal date time string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.String.</returns>
        /// <remarks>no remarks</remarks>
        public static string ToUniversalDateTimeString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(UniversalDateTimeFormat, CultureInfo.InvariantCulture);
            //return dateTime.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The universal date time format
        /// </summary>
        private const string UniversalDateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";


        /// <summary>
        /// Convert the specified value to a boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks>Always succeeds, returning 'false' if the value could not be interpreted as a 'true' value.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static bool Bool(string value)
        {
            if (0 == string.CompareOrdinal(value, Boolean.TrueString))
                return true;
            var n = 0;
            if (int.TryParse(value, out n))
            {
                return (n != 0);
            }
            //-- Test using string representations of 'true'
            if (value == null)
                return false;
            switch (value.ToUpperInvariant().Trim())
            {
                case "AAN":
                case "ON":
                case "T":
                case "TRUE":
                case "1":
                case "VISIBLE":
                case "ZICHTBAAR":
                case "WAAR":
                case "JA":
                case "YES":
                case "Y":
                case "J":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Hexadecimals the string to bytes.
        /// </summary>
        /// <param name="dataIn">The data in.</param>
        /// <returns>System.Byte[].</returns>
        /// <remarks>no comments</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.Parse(System.String,System.Globalization.NumberStyles)")]
        public static byte[] HexStringToBytes(string dataIn)
        {
            if (dataIn == null) return new byte[] { };
            dataIn = dataIn.Replace("-", "");
            // Debug.Assert(value.Length % 2 == 0);

            var result = new byte[dataIn.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(dataIn.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return result;
        }

        /// <summary>
        /// Convert the string value to a date object
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.DateTime.</returns>
        /// <exception cref="System.ArgumentException">Invalid date :  + dateValue</exception>
        /// <exception cref="ArgumentException">Invalid date :  + dateValue</exception>
        /// <remarks>no remarks</remarks>
        public static System.DateTime Date(string value)
        {
            if (string.IsNullOrEmpty(value))
                return System.DateTime.MinValue;
            string dateValue = value.Trim();
            //-- controleer of datum is opgegeven in de string
            Regex regexp = new Regex("^[A]?[0-9]*$", RegexOptions.IgnoreCase);
            string d = null;
            System.DateTime d2 = default(System.DateTime);
            if (regexp.Match(dateValue).Success)
            {
                //-- de invoer bestaat alleen uit nummers
                //-- gebruik dag/maand/jaar (huidige localiteit)
                //-- of ansidate?
                if (dateValue.StartsWith("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    string m = null;
                    string y = null;
                    y = dateValue.Substring(1, 4);
                    m = dateValue.Substring(5, 2);
                    d = dateValue.Substring(7, 2);
                    try
                    {
                        return new DateTime(int.Parse(y, CultureInfo.InvariantCulture), int.Parse(m, CultureInfo.InvariantCulture), int.Parse(d, CultureInfo.InvariantCulture));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //-- er zijn ongeldige gegevens voor jaar/maand/dag opgegeven
                        return System.DateTime.MinValue;
                    }
                }
                else
                {
                    //-- Als lengte=8 dan ANSI
                    if (dateValue.Length == 8)
                    {
                        return AnsiDate(value);
                    }
                    else
                    {
                        var i = 0;
                        if (int.TryParse(dateValue, out i))
                        {
                            DateTime nowDate = DateTime.Now;
                            d = nowDate.Day.ToString(CultureInfo.InvariantCulture);
                            string m = nowDate.Month.ToString(CultureInfo.InvariantCulture);
                            string y = nowDate.Year.ToString(CultureInfo.InvariantCulture);

                            //-- Mooi principe, maar we gaan uit van 
                            //   NL format: DMY
                            //Dim df As string = SysInfo.DateFormat
                            const string df = "DMY";
                            //-- string op de juiste lengte maken

                            if ((dateValue.Length == 1) || (dateValue.Length == 3) || (dateValue.Length == 5))
                            {
                                dateValue = "0" + dateValue;
                            }

                            switch (dateValue.Length)
                            {
                                case 2:
                                    d = dateValue.Substring(0, 2);
                                    break;
                                case 4:
                                    switch (df)
                                    {
                                        case "DMY":
                                        case "YDM":
                                        case "DYM":
                                            //-- dag voor maand
                                            d = dateValue.Substring(0, 2);
                                            m = dateValue.Substring(2, 2);
                                            break;
                                    }
                                    break;
                                case 6:
                                    switch (df)
                                    {
                                        case "DMY":
                                            d = dateValue.Substring(0, 2);
                                            m = dateValue.Substring(2, 2);
                                            y = string.Concat(y.Substring(0, 2), dateValue.Substring(4, 2));
                                            break;
                                    }
                                    break;
                                case 8:
                                    switch (df)
                                    {
                                        case "DMY":
                                            d = dateValue.Substring(0, 2);
                                            m = dateValue.Substring(2, 2);
                                            y = dateValue.Substring(4, 4);
                                            break;
                                    }
                                    break;
                                default:
                                    throw new ArgumentException("Invalid date : " + dateValue);
                            }

                            return new DateTime(int.Parse(y, CultureInfo.InvariantCulture), int.Parse(m, CultureInfo.InvariantCulture), int.Parse(d, CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            return System.DateTime.MinValue;
                        }
                    }
                }
            }
            else
            {
                var nl = new CultureInfo("nl-nl");

                if (DateTime.TryParse(dateValue.Replace('/', '-'), nl, DateTimeStyles.AssumeLocal, out d2))
                {
                    //-- geen tijden meenemen
                    return new DateTime(d2.Year, d2.Month, d2.Day);
                }
                else
                {
                    // standaard datum
                    if (DateTime.TryParse(dateValue, out d2)) return new DateTime(d2.Year, d2.Month, d2.Day);
                    //-- test op datum +1, -1 etc
                    dateValue = dateValue.Trim();
                    var dt = DateTime.Now;
                    if (dateValue.StartsWith("datum", StringComparison.OrdinalIgnoreCase))
                    {
                        //-- Optellen van datum
                        var addDate = dateValue.Substring("datum".Length).Trim();
                        double dn = 0;
                        d2 = double.TryParse(addDate, out dn)
                            ? new DateTime(dt.Year, dt.Month, dt.Day).AddDays(dn)
                            : new DateTime(dt.Year, dt.Month, dt.Day);
                    }
                    else if (string.Compare("vandaag", dateValue, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        d2 = new DateTime(dt.Year, dt.Month, dt.Day);
                    }
                    else
                    {
                        d2 = System.DateTime.MinValue;
                    }
                    return d2;
                }
            }
        }

        /// <summary>
        /// Convert the string value to a date, assuming a format with 'yyyymmdd'
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.DateTime.</returns>
        /// <remarks>no remarks</remarks>
        public static System.DateTime AnsiDate(string value)
        {
            return DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }


        /// <summary>
        /// Convert an expression property to it's string representation
        /// <code type="c#">
        /// var propertName = PropertyToString&lt;MyClass&gt;( o =&gt; o.MyProperty );
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">property expression</param>
        /// <returns>System.String.</returns>
        /// <remarks>no remarks</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string PropertyToString<T>(Expression<Func<T, object>> property)
        {
            if (property == null)
                return string.Empty;

            var body = property.Body as UnaryExpression;

            var memberExpression = body == null ?
                property.Body as MemberExpression : // string property
                body.Operand as MemberExpression;

            return memberExpression == null ?
                string.Empty : // Coen: niet kunnen testen (dus maar 93.33% test code coverage), kan op 
                               //       dit moment geen situatie verzinnen wanneer dit voor komt.
                memberExpression.Member.Name;
        }
    }
}
