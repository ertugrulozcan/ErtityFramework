using System;
using System.ComponentModel;

namespace ErtityFramework.Helpers
{
    public static class GenericParser
    {
        public static T2 Parse<T2>(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(T2);
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T2));
                if (converter != null)
                {
                    return (T2)converter.ConvertFromString(input);
                }

                return default(T2);
            }
            catch (NotSupportedException)
            {
                return default(T2);
            }
        }

        public static object Parse(string input, Type type)
        {
            if (string.IsNullOrEmpty(input))
            {
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                else
                    return null;
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    return converter.ConvertFromString(input);
                }

                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }

                return null;
            }
            catch (NotSupportedException)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }

                return null;
            }
        }
    }
}
