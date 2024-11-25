using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moinsa.Arcante.Company.Business.Validations
{
    public static class Guard
    {
        public static void CheckIsNotNull(object target, string parametername, string message = "'{0}' can't be null.")
        {
            if (target == null)
            {
                throw new ArgumentNullException(string.Format(message, parametername), parametername);
            }
        }

        public static void CheckIsNotNullOrWhiteSpace(string target, string parametername, string message = "'{0}' can't be empty.")
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentNullException(string.Format(message, parametername), parametername);
            }
        }

        public static void CheckIsNumber(string target, string parametername, string message = "'{0}' only accept numbers")
        {
            CheckIsNotNullOrWhiteSpace(target, parametername, message);
            if (!target.All(char.IsDigit))
            {
                throw new ArgumentNullException(string.Format(message, parametername), parametername);
            }
        }

        public static void CheckValue(object target, string parametername, object value, string message = "'{0}' must have the value '{1}'.")
        {
            if (!target.Equals(value))
            {
                throw new ArgumentException(string.Format(message, parametername, value), parametername);
            }
        }

        public static void CheckIsTrue(object target, string parametername, string message = "'{0}' must have the value 'true'.")
        {
            CheckValue(target, parametername, true, message);
        }

        public static void CheckIsFalse(object target, string parametername, string message = "'{0}' must have the value 'false'.")
        {
            CheckValue(target, parametername, false, message);
        }

        public static void CheckNotIn(object target, string parametername, string message = "'{0}' can't has value {1}. Only the values {2} are accepted.", params object[] values)
        {
            foreach (object obj in values)
            {
                if (target.Equals(obj))
                {
                    throw new ArgumentException(string.Format(message, parametername, obj, string.Join(",", values)), parametername);
                }
            }
        }

        public static void CheckGreaterThan(DateTime? target, string parametername, DateTime minValue, string message = "'{0}' must be greater than {1}.")
        {
            if (target.HasValue)
            {
                CheckGreaterThan(target.Value, parametername, minValue, message);
                return;
            }

            throw new ArgumentException(string.Format(message, parametername, minValue), parametername);
        }

        public static void CheckGreaterThan(DateTime target, string parametername, DateTime minValue, string message = "'{0}' must be greater than {1}.")
        {
            if (target <= minValue)
            {
                throw new ArgumentException(string.Format(message, parametername, minValue), parametername);
            }
        }

        public static void CheckGreaterThan(int target, string parametername, int minValue, string message = "'{0}' must be greater than {1}.")
        {
            if (target <= minValue)
            {
                throw new ArgumentException(string.Format(message, parametername, minValue), parametername);
            }
        }

        public static void CheckLength(string target, string parametername, int length, string message = "The length of '{0}' must be {1}.")
        {
            if (target.Length == length)
            {
                throw new ArgumentException(string.Format(message, parametername, length), parametername);
            }
        }

        public static void CheckLengthGreaterOrEqualThan(string target, string parametername, int minValue, string message = "The length of '{0}' must be greater or equal than {1}.")
        {
            if (target.Length <= minValue)
            {
                throw new ArgumentException(string.Format(message, parametername, minValue), parametername);
            }
        }

        public static void CheckListIsNotEmpty<T>(List<T> target, string parametername, string message = "'{0}' must contain at least one item.")
        {
            CheckIsNotNull(target, parametername, message);
            if (target.Count == 0)
            {
                throw new ArgumentException(string.Format(message, parametername), parametername);
            }
        }

        public static void CheckEntityExits(object target, string message)
        {
            if (target == null)
            {
                throw new ArgumentException(message);
            }
        }

        public static void CheckEntityExits(object target, string entityname, Dictionary<string, string> keyValuePair, string message = "Entity '{0}' with keys {1} not found in Database.")
        {
            CheckEntityExits(target, string.Format(message, entityname, System.Text.Json.JsonSerializer.Serialize((object)keyValuePair)));
        }

        public static void CheckEntityExits(int target, string entityname, string id, string message = "Entity '{0}' with id {1} not found in Database.")
        {
            if (target <= 0)
            {
                throw new ArgumentException(string.Format(message, entityname, id), entityname);
            }
        }

        public static void CheckEntityExits(int target, string entityname, int id, string message = "Entity '{0}' with id {1} not found in Database.")
        {
            CheckEntityExits(target, entityname, id.ToString(), message);
        }
    }
}
