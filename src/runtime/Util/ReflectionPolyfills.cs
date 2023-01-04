using System;
using System.Linq;
using System.Reflection;

namespace Python.Runtime
{
    internal static class ReflectionPolyfills
    {
        public static T GetCustomAttribute<T>(this Type type) where T: Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit: false)
                .Cast<T>()
                .SingleOrDefault();
        }

        public static T GetCustomAttribute<T>(this Assembly assembly) where T: Attribute
        {
            return assembly.GetCustomAttributes(typeof(T), inherit: false)
                .Cast<T>()
                .SingleOrDefault();
        }

        public static bool IsFlagsEnum(this Type type)
            => type.GetCustomAttribute<FlagsAttribute>() is not null;
    }
}
