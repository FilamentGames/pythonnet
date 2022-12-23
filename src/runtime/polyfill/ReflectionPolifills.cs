using System;
using System.Linq;
using System.Reflection;

namespace Python.Runtime
{
    [Obsolete("This API is for internal use only")]
    public static class ReflectionPolifills
    {
#if NETSTANDARD
        public static AssemblyBuilder DefineDynamicAssembly(this AppDomain appDomain, AssemblyName assemblyName, AssemblyBuilderAccess assemblyBuilderAccess)
        {
            return AssemblyBuilder.DefineDynamicAssembly(assemblyName, assemblyBuilderAccess);
        }

        public static Type CreateType(this TypeBuilder typeBuilder)
        {
            return typeBuilder.GetTypeInfo().GetType();
        }
#endif
        public static T GetCustomAttribute<T>(this Type type) where T: Attribute
        {
            // GetCustomAttributes not supported by IL2CPP
            return default(T);
        }

        public static T GetCustomAttribute<T>(this Assembly assembly) where T: Attribute
        {
            // GetCustomAttributes not supported by IL2CPP
            return default(T);
        }
    }
}
