using System.Net;

namespace Mc.Core.Localization
{
    /// <summary>
    /// Class Local contains extensions for localization
    /// </summary>
    /// <remarks>no comments</remarks>
    public static class Messages
    {
        /// <summary>
        /// Return the user message for the http statuscode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        /// <remarks>no comments</remarks>
        public static string UserMessage(this HttpStatusCode message)
        {
            // return CoreMessages.ResourceManager.GetString(message.ToString(),CultureInfo.CurrentCulture)??string.Empty;
            return message.ToString();
        }
    }

}
