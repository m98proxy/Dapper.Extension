using System.Linq;
using System.Reflection;

namespace Dapper
{
    static class PropertyInfoExtensions
    {
        public static bool HasAttribute(this PropertyInfo propertyInfo, string attributeName)
        {
            return propertyInfo.GetCustomAttributes(true).Any(attr => attr.GetType().Name == attributeName);
        }
    }
}
