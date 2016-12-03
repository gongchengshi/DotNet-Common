using System;
using System.Reflection;

namespace Gongchengshi
{
    public static class ReflectionHelpers
    {
        public static object CreateObject(string name, params object[] constructorArgs)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] exportedTypes;

                try
                {
                    exportedTypes = asm.GetExportedTypes();
                }
                catch (NotSupportedException)
                {
                    continue;
                }

                foreach (Type type in exportedTypes) //GetTypes())
                {
                    if (type.FullName.Contains(name))
                    {
                        object o = Activator.CreateInstance(type, constructorArgs);
                        return o;
                    }
                }
            }
            return null;
        }
    }
}