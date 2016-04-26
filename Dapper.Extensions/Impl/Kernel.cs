using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper
{
    static class Kernel
    {
        private static Dialect _dialect;

        private static string _wrapper;

        public static void Log(string format, params object[] args)
        {
            if (Debugger.IsAttached)
            {
                Trace.WriteLine(string.Concat("> ", string.Format(format, args)));
            }
        }

        public static void SetDialect(Dialect dialect)
        {
            switch (dialect)
            {
                default:

                    _dialect = Dialect.MSSQL;

                    _wrapper = "[{0}]";

                    break;

                case Dialect.Postgre:

                    _dialect = Dialect.Postgre;

                    _wrapper = "`{0}`";

                    break;

                case Dialect.SQLite:

                    _dialect = Dialect.SQLite;

                    _wrapper = "`{0}`";

                    break;

                case Dialect.MySQL:

                    _dialect = Dialect.MySQL;

                    _wrapper = "`{0}`";

                    break;
            }
        }

        public static string WrapUp(string input)
        {
            return string.Format(_wrapper, input);
        }

        public static IEnumerable<PropertyInfo> GetKeyProperties(Type type)
        {
            var props = type.GetPropertiesWithAttribute<KeyAttribute>();

            return props.Any() ? props : null;
        }

        public static void CheckForKeyProperty(IEnumerable<PropertyInfo> properties)
        {
            Protect.Against(!properties.Any(), "Could not find a [Key] property");
        }

        public static void CheckForKeyProperty<TEntity>()
        {
            var propriedades = Kernel.GetKeyProperties(typeof(TEntity));

            CheckForKeyProperty(propriedades);
        }

        public static string GetTableName(Type type)
        {
            var tableName = WrapUp(type.Name);

            var tableAttribute = type.GetAttribute<TableAttribute>();

            if (tableAttribute != null)
            {
                tableName = WrapUp(tableAttribute.Name);

                Log("table name override from {0} to {1}", type.Name, tableName);

                if (!string.IsNullOrWhiteSpace(tableAttribute.Schema))
                {
                    string schemaName = WrapUp(tableAttribute.Schema);

                    tableName = string.Format("{0}.{1}", schemaName, tableName);

                    Log("table name override from {0} to {1}", type.Name, tableName);
                }
            }

            return tableName;
        }

        public static string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }

        public static string GetColumnName(PropertyInfo property)
        {
            var columnName = WrapUp(property.Name);

            var columnAttribute = property.GetAttribute<ColumnAttribute>();

            if (columnAttribute != null)
            {
                columnName = WrapUp(columnAttribute.Name);

                Log("column name override from {0} to {1}", property.Name, columnName);
            }

            return columnName;
        }









        public static string BuildSelectContent<TEntity>()
        {
            var properties = typeof(TEntity).GetProperties();

            var buffer = new StringBuilder();

            var addedColumnCounter = 0;

            foreach (var prop in properties)
            {
                if (prop.HasAttribute<NotMappedAttribute>()) continue;

                if (addedColumnCounter > 0) buffer.Append(",");

                buffer.Append(GetColumnName(prop));

                if (prop.HasAttribute<ColumnAttribute>())
                {
                    buffer.Append(string.Concat(" as ", prop.Name));
                }

                ++addedColumnCounter;
            }

            return buffer.ToString();
        }

        public static string BuildUpdateContent<T>(string parameterChar = null)
        {
            var properties = typeof(T).GetEditableProperties();

            var buffer = new StringBuilder();

            var addedColumnCounter = 0;

            parameterChar = parameterChar ?? "@";

            foreach (var prop in properties)
            {
                if (prop.HasAttribute<NotMappedAttribute>()) continue;

                if (addedColumnCounter > 0) buffer.Append(",");

                buffer.AppendFormat("{0} = {1}{2}", GetColumnName(prop), parameterChar, prop.Name);

                ++addedColumnCounter;
            }

            return buffer.ToString();
        }
    }
}
