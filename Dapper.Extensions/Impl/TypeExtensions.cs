using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dapper
{
    static class TypeExtension
    {
        public static bool IsSimpleType(this Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var simpleTypesList = new List<Type>
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(bool),
                typeof(string),
                typeof(char),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(byte[])
            };

            return simpleTypesList.Contains(type) || type.IsEnum;
        }

        public static bool HasAttribute(this Type type, string attributeName)
        {
            return type.GetCustomAttributes(true).Any(attr => attr.GetType().Name == attributeName);
        }

        public static IEnumerable<PropertyInfo> ObterPropriedades(this Type type, string attributeName)
        {
            return type.GetProperties().Where(p => p.HasAttribute(attributeName));
        }

        public static IEnumerable<PropertyInfo> ObterPropriedades(this Type tipo, Func<PropertyInfo, bool> predicado)
        {
            return tipo.GetProperties().Where(predicado);
        }
    }
}
