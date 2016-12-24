using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mc.Core
{
    /// <summary>
    /// Class CLI takes an argument list and provides functionality for Command Line Interpretation
    /// <example>
    /// 
    /// </example>
    /// </summary>
    /// <remarks>no comments</remarks>
    // ReSharper disable once InconsistentNaming
    public class CLI
    {
        private static readonly Regex ArgRegex = new Regex(@"^[/\-]?(?<name>[^:=]+)(?:[:=](?<value>.*))?$");
        private readonly Dictionary<string, string> _instructions;

        /// <summary>
        /// Gets the last exception.
        /// </summary>
        /// <value>The last exception.</value>
        /// <remarks>no comments</remarks>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CLI"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <remarks>no comments</remarks>
        public CLI(IEnumerable<string> args)
        {
            _instructions = new Dictionary<string, string>();
            if (args == null) return;
            foreach (var arg in args)
            {
                var m = ArgRegex.Match(arg);
                if (m.Success)
                {
                    var name = m.Groups["name"].Value.ToUpperInvariant();
                    var value = m.Groups["value"].Value;
                    _instructions[name] = value;
                }
            }
        }


        /// <summary>
        /// Determines whether the specified name is available as an instruction.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns><c>true</c> if the specified instruction was found; otherwise, <c>false</c>.</returns>
        /// <remarks>no comments</remarks>
        public bool HasInstruction(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            name = name.ToUpperInvariant();
            return _instructions.ContainsKey(name);
        }

        /// <summary>
        /// Gets the instruction with the optional parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="result">The parameter value.</param>
        /// <returns>System.String.</returns>
        /// <remarks>no comments</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool GetInstruction<T>(string name, out T result)
        {
            result = default(T);
            if (string.IsNullOrEmpty(name)) return false;
            name = name.ToUpperInvariant();
            if (!_instructions.ContainsKey(name)) return false;
            try
            {
                var value = _instructions[name];
                if (typeof(T) == typeof(DateTime))
                {
                    if (IsOf.AnsiDate(value))
                    {
                        var dateResult = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
                        result = (T)Convert.ChangeType(dateResult, typeof(T), CultureInfo.InvariantCulture);
                        return true;
                    }
                }
                if (typeof(T) == typeof(bool))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        result = (T)Convert.ChangeType(true, typeof(T), CultureInfo.InvariantCulture);
                        return true;
                    }
                    result = (T)Convert.ChangeType(ConvertTo.Bool(value), typeof(T), CultureInfo.InvariantCulture);
                    return true;
                }
                result = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                LastException = e;
            }
            return false;
        }
    }
}
