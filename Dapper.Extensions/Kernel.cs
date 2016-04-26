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

        private static string _parameterChar;

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

                    _parameterChar = "@";

                    break;

                case Dialect.Oracle:

                    _dialect = Dialect.Oracle;

                    _wrapper = "{0}";

                    _parameterChar = ":";

                    break;

                case Dialect.Postgre:

                    _dialect = Dialect.Postgre;

                    _wrapper = "{0}";

                    _parameterChar = "@";

                    break;

                case Dialect.SQLite:

                    _dialect = Dialect.SQLite;

                    _wrapper = "{0}";

                    _parameterChar = "@";

                    break;

                case Dialect.MySQL:

                    _dialect = Dialect.MySQL;

                    _wrapper = "`{0}`";

                    _parameterChar = "@";

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

        public static string BuildStatementContent<TEntity>(IEnumerable<PropertyInfo> properties, Action<StringBuilder, PropertyInfo> builder)
        {
            var buffer = new StringBuilder();

            var addedColumnCounter = 0;

            foreach (var property in properties)
            {
                if (property.HasAttribute<NotMappedAttribute>()) continue;

                if (addedColumnCounter > 0) buffer.Append(", ");

                builder(buffer, property);

                ++addedColumnCounter;
            }

            if (buffer.ToString().Trim().EndsWith(","))
            {
                buffer.Remove(buffer.Length - 1, 1);
            }

            return buffer.ToString();
        }

        public static string BuildSelectStatementContent<TEntity>()
        {
            var properties = typeof(TEntity).GetProperties();

            return BuildStatementContent<TEntity>(properties, (buffer, property) =>
            {
                buffer.Append(GetColumnName(property));

                if (property.HasAttribute<ColumnAttribute>())
                {
                    buffer.Append(string.Concat(" as ", property.Name));
                }
            });
        }

        public static string BuildUpdateStatementContent<TEntity>(bool includeKeyProperty = false)
        {
            var properties = typeof(TEntity).GetEditableProperties(includeKeyProperty: includeKeyProperty);

            return BuildStatementContent<TEntity>(properties, (buffer, property) =>
            {
                buffer.AppendFormat("{0} = {1}{2}", GetColumnName(property), _parameterChar, property.Name);
            });
        }

        public static string BuildInsertStatementContent<TEntity>(bool includeKeyProperty = false)
        {
            var properties = typeof(TEntity).GetEditableProperties(includeKeyProperty: includeKeyProperty);

            return BuildStatementContent<TEntity>(properties, (buffer, property) =>
            {
                buffer.Append(GetColumnName(property));
            });
        }

        public static string BuildInsertStatementValueContent<TEntity>(bool includeKeyProperty = false)
        {
            var properties = typeof(TEntity).GetEditableProperties(includeKeyProperty: includeKeyProperty);

            return BuildStatementContent<TEntity>(properties, (buffer, property) =>
            {
                buffer.AppendFormat("{0}{1}", _parameterChar, property.Name);
            });
        }

        public static string BuildWhereClause<TEntity>(object condition)
        {
            var buffer = new StringBuilder();

            var source = typeof(TEntity).GetEditableProperties(includeKeyProperty: true);

            var addedColumnCounter = 0;

            var criteria = condition.GetType();            

            foreach (var property in source)
            {
                var criterion = criteria.GetProperty(property.Name);

                if (criterion != null)
                {
                    var value = criterion.GetValue(condition, null);

                    var format = (value == null || value == DBNull.Value) ? "{0} is null" : "{0} = {1}{2}";

                    if (addedColumnCounter > 0) buffer.Append(" and ");

                    buffer.AppendFormat(format, GetColumnName(property), property.Name);

                    ++addedColumnCounter;
                }
            }

            if (buffer.ToString().EndsWith(" and "))
            {
                buffer.Remove(buffer.Length - 5, 5);
            }

            var clause = buffer.ToString();

            return (!string.IsNullOrWhiteSpace(clause)) ? string.Format("where {0}", clause) : string.Empty;
        }
    }
}
