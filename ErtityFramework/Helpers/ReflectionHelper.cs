using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

namespace ErtityFramework.Helpers
{
    public static class ReflectionHelper
    {
        public static T CreateInstance<T>()
        {
            if (typeof(T).IsValueType)
            {
                return default(T);
            }
            else
            {
                var ctors = typeof(T).GetConstructors();
                if (ctors.Any(x => x.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance<T>();
                }
                else
                {
                    foreach (var ctor in ctors)
                    {
                        try
                        {
                            var parameterInfos = ctor.GetParameters();
                            object[] parameters = new object[parameterInfos.Length];
                            int i = 0;
                            foreach (var pInfo in parameterInfos)
                            {
                                parameters[i] = CreateInstance(pInfo.ParameterType);
                                i++;
                            }

                            if (parameters.Length > 0)
                                return (T)ctor.Invoke(parameters);
                            else
                                return (T)ctor.Invoke(null);
                        }
                        finally
                        {

                        }
                    }

                    return default(T);
                }
            }
        }

        public static T CreateInstance<T>(object[] parameters)
        {
            if (typeof(T).IsValueType)
            {
                return default(T);
            }
            else
            {
                var ctors = typeof(T).GetConstructors();
                if (ctors.Any(x => x.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance<T>();
                }
                else
                {
                    foreach (var ctor in ctors)
                    {
                        try
                        {
                            if (parameters != null && parameters.Length > 0)
                                return (T)ctor.Invoke(parameters);
                            else
                                return (T)ctor.Invoke(null);
                        }
                        finally
                        {

                        }
                    }

                    return default(T);
                }
            }
        }

        public static object CreateInstance(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var ctors = type.GetConstructors();
                if (ctors.Any(x => x.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance(type);
                }
                else
                {
                    foreach (var ctor in ctors)
                    {
                        try
                        {
                            var parameterInfos = ctor.GetParameters();
                            object[] parameters = new object[parameterInfos.Length];
                            int i = 0;
                            foreach (var pInfo in parameterInfos)
                            {
                                parameters[i] = CreateInstance(pInfo.ParameterType);
                                i++;
                            }

                            if (parameters != null && parameters.Length > 0)
                                return ctor.Invoke(parameters);
                            else
                                return ctor.Invoke(null);
                        }
                        finally
                        {

                        }
                    }

                    return null;
                }
            }
        }

        public static object CreateInstance(Type type, object[] parameters)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var ctors = type.GetConstructors();
                if (ctors.Any(x => x.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance(type);
                }
                else
                {
                    foreach (var ctor in ctors)
                    {
                        try
                        {
                            if (parameters != null && parameters.Length > 0)
                                return ctor.Invoke(parameters);
                            else
                                return ctor.Invoke(null);
                        }
                        finally
                        {

                        }
                    }

                    return null;
                }
            }
        }

        public static void SetProperties(object obj, Dictionary<string, object> properties)
        {
            foreach (var pair in properties)
            {
                PropertyInfo prop = obj.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var propertyType = prop.PropertyType.GetGenericArguments()[0];

                        if (propertyType.IsEnum)
                        {
                            var enumValue = Enum.Parse(propertyType, pair.Value.ToString());
                            prop.SetValue(obj, enumValue, null);
                        }
                        else
                        {
                            Type t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                            object safeValue = (pair.Value == null) ? null : Convert.ChangeType(pair.Value, t);

                            prop.SetValue(obj, safeValue, null);
                        }
                    }
                    else
                    {
                        if (prop.PropertyType.IsEnum)
                        {
                            var enumValue = Enum.Parse(prop.PropertyType, pair.Value.ToString());
                            prop.SetValue(obj, enumValue, null);
                        }
                        else
                        {
                            prop.SetValue(obj, pair.Value, null);
                        }
                    }
                }
            }
        }
    }
}
