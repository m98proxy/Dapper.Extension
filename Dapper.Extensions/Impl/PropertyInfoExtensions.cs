using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dapper
{
    static class PropertyInfoExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : class
        {
            var attributeName = typeof(TAttribute).Name;

            return propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == attributeName) as TAttribute;
        }

        public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo)
        {
            var attributeName = typeof(TAttribute).Name;

            return propertyInfo.GetCustomAttributes(true).Any(attr => attr.GetType().Name == attributeName);
        }

        public static bool IsEditable(this PropertyInfo property)
        {
            var attribute = property.GetAttribute<EditableAttribute>();

            return (attribute != null) ? attribute.AllowEdit : false;
        }

        public static bool IsReadOnly(this PropertyInfo property)
        {
            var attribute = property.GetAttribute<ReadOnlyAttribute>();

            return (attribute != null) ? attribute.IsReadOnly : false;
        }

        public static bool IsKey(this PropertyInfo property)
        {
            var attribute = property.GetAttribute<KeyAttribute>();

            return (attribute != null);
        }

        public static bool IsMapped(this PropertyInfo property)
        {
            var attribute = property.GetAttribute<NotMappedAttribute>();

            return (attribute == null);
        }
    }
}
