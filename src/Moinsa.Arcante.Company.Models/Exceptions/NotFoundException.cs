using System;
using System.Net;

namespace API.Company.Models.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(message, null, HttpStatusCode.NotFound)
        {
        }

        /// <summary>
        /// Throw an exception if the condition is false
        /// </summary>
        /// <param name="message"></param>
        /// <param name="predicate"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <example>
        /// ThrowIfNotFound(matricula, () => entity != null);
        /// </example>
        public static void ThrowIfCondition(string message, Func<bool> predicate)
        {
            ThrowIfCondition(message, predicate());
        }

        /// <summary>
        /// Throw an exception if the condition is false
        /// </summary>
        /// <param name="message"></param>
        /// <param name="condition"></param>
        /// <exception cref="NotFoundException"></exception>
        public static void ThrowIfCondition(string message, bool condition)
        {
            if (condition)
            {
                throw new NotFoundException(message);
            }
        }
    }

}