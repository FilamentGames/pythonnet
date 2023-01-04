using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Python.Runtime
{
    /// <summary>
    /// Several places in the runtime generate code on the fly to support
    /// dynamic functionality. The CodeGenerator class manages the dynamic
    /// assembly used for code generation and provides utility methods for
    /// certain repetitive tasks.
    /// </summary>
    internal class CodeGenerator
    {

        const string NamePrefix = "__Python_Runtime_Generated_";

        internal CodeGenerator()
        {
        }

        static string GetUniqueAssemblyName(string name)
        {
            var taken = new HashSet<string>(AppDomain.CurrentDomain
                                                     .GetAssemblies()
                                                     .Select(a => a.GetName().Name));
            for (int i = 0; i < int.MaxValue; i++)
            {
                string candidate = name + i.ToString(CultureInfo.InvariantCulture);
                if (!taken.Contains(candidate))
                    return candidate;
            }

            throw new NotSupportedException("Too many assemblies");
        }
    }
}
