using System;
using System.Globalization;
using System.Net;
using Mc.Core.Localization;

namespace Mc.Core
{
    /// <summary>
    /// Class Validate contains parameter validations.
    /// </summary>
    /// <remarks>no comments</remarks>
    public static class Validate
    {

        /// <summary>
        /// Determines whether the specified string is in an ANSI date format
        /// </summary>
        /// <param name="value">The string date.</param>
        /// <returns><c>true</c> if [is ANSI date] [the specified string date]; otherwise, <c>false</c>.</returns>
        /// <remarks>no comments</remarks>
        public static bool IsAnsiDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value.Length != 8) return false;
            DateTime date;
            return DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        /// <summary>
        /// Check the item, throw an exception if it is null or not an ANSI date format
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The string date.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks>no comments</remarks>
        public static void AnsiDate(string parameterName, string value)
        {
            if (!IsAnsiDate(value)) throw new ArgumentOutOfRangeException(parameterName, Messages.UserMessage(HttpStatusCode.NotAcceptable));
        }

        /// <summary>
        /// Check the item, throw an exception if it is null
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <remarks>no comments</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static void NotNull(string parameterName, [ValidatedNotNull] object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(parameterName, Messages.UserMessage(HttpStatusCode.NoContent));
            }
            if (item is string)
            {
                var s = item as string;
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentNullException(parameterName, Messages.UserMessage(HttpStatusCode.NoContent));
            }
        }

        /// <summary>
        /// Determines whether the specified item is not null.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified item is not null; otherwise, <c>false</c>.</returns>
        /// <remarks>no comments</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static bool IsNotNull(object item)
        {
            if (item == null)
            {
                return false;
            }
            if (item is string)
            {
                var s = item as string;
                if (string.IsNullOrEmpty(s)) return false;
            }
            return true;
        }

        /// <summary>
        /// Check the item, throw an exception if it represents an empty guid
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <remarks>no comments</remarks>
        public static void NotNull(string parameterName, Guid item)
        {
            if (item == Guid.Empty)
            {
                throw new ArgumentNullException(parameterName, Messages.UserMessage(HttpStatusCode.NoContent));
            }
        }

        /// <summary>
        /// Determines whether the specified item is not null.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified GUID item is not null; otherwise, <c>false</c>.</returns>
        /// <remarks>no comments</remarks>
        public static bool IsNotNull(Guid item)
        {
            if (item == Guid.Empty)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the item, throw an exception if the value is less than or equal to zero
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>no comments</remarks>
        public static void Positive(string parameterName, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, Messages.UserMessage(HttpStatusCode.NotAcceptable));
            }
        }

        /// <summary>
        /// Check the item, throw an exception if the value is less than or equal to zero
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>no comments</remarks>
        public static void Positive(string parameterName, decimal value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, Messages.UserMessage(HttpStatusCode.NotAcceptable));
            }
        }

        /// <summary>
        /// Check the item, throw an exception if the value is outside the range
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="endValue">The end value.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>no comments</remarks>
        public static void Range(string parameterName, decimal value, decimal startValue, decimal endValue)
        {
            if (value < startValue || value > endValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, Messages.UserMessage(HttpStatusCode.NotAcceptable));
            }
        }

        /// <summary>
        /// Check the item, throw an exception if the value is outside the range
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="endValue">The end value.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>no comments</remarks>
        public static void Range(string parameterName, int value, int startValue, int endValue)
        {
            if (value < startValue || value > endValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, Messages.UserMessage(HttpStatusCode.NotAcceptable));
            }
        }

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
