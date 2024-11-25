using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.ComponentModel.DataAnnotations
{
    public static class DateTimeExtensions

    {
        public static DateTime FirstOfMonth(this DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

        public static DateTime FirstOfYear(this DateTime dateTime)
                    => new DateTime(dateTime.Year, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

        public static DateTime LastOfMonth(this DateTime dateTime)
            => new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

        public static DateTime LastOfYear(this DateTime dateTime)
                    => new DateTime(dateTime.Year, 12, 31, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
    }

    /// <see cref="http://www.tempesta.space/Link/a-flexible-default-value-for-your-datetime-properties"/>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DefaultDateTimeValueAttribute : ValidationAttribute
    {
        public DefaultDateTimeValueAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public string DefaultValue { get; set; }
        private bool IsExtensionMethod
            => typeof(DefaultDateTimeValueAttribute).Assembly
                .GetType(typeof(DateTimeExtensions).FullName)
                .GetMethods()
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Select(p => p.Name).Contains(this.DefaultValue);

        private bool IsProperty
            => typeof(DateTime).GetProperties()
                .Select(p => p.Name).Contains(this.DefaultValue);

        private bool IsRelativeValue
            => this.DefaultValue.Contains(":");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            
            // Set default value only if no value is already specified
            if (value == null)
            {
                DateTime defaultValue = GetDefaultValue();
                property.SetValue(validationContext.ObjectInstance, defaultValue);
            }

            return ValidationResult.Success;
        }

        private DateTime GetAbsoluteValue()
        {
            DateTime value;

            if (!DateTime.TryParse(this.DefaultValue, out value))
            {
                return default(DateTime);
            }

            return value;
        }

        private DateTime GetDefaultValue()
        {
            // Resolve a named property of DateTime, like "Now"
            if (this.IsProperty)
            {
                return GetPropertyValue();
            }

            // Resolve a named extension method of DateTime, like "LastOfMonth"
            if (this.IsExtensionMethod)
            {
                return GetExtensionMethodValue();
            }

            // Parse a relative date
            if (this.IsRelativeValue)
            {
                return GetRelativeValue();
            }

            // Parse an absolute date
            return GetAbsoluteValue();
        }
        private DateTime GetExtensionMethodValue()
        {
            var instance = Activator.CreateInstance<DateTime>();

            var value = (DateTime)typeof(DefaultDateTimeValueAttribute).Assembly
                .GetType(typeof(DateTimeExtensions).FullName)
                .GetMethod(this.DefaultValue)
                .Invoke(instance, new object[] { DateTime.Now });

            return value;
        }

        private DateTime GetPropertyValue()
        {
            var instance = Activator.CreateInstance<DateTime>();
            var value = (DateTime)instance.GetType()
                .GetProperty(this.DefaultValue)
                .GetValue(instance);

            return value;
        }
        private DateTime GetRelativeValue()
        {
            TimeSpan timeSpan;

            if (!TimeSpan.TryParse(this.DefaultValue, out timeSpan))
            {
                return default(DateTime);
            }

            return DateTime.Now.Add(timeSpan);
        }
    }
}