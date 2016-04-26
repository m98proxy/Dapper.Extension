using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    static class StatementFactory
    {
        public static string Select<TEntity>(string condition)
        {
            Kernel.CheckForKeyProperty<TEntity>();

            string selectStatementContent = Kernel.BuildSelectStatementContent<TEntity>();

            string tableName = Kernel.GetTableName<TEntity>();

            string whereClause = Kernel.BuildWhereClause<TEntity>(condition);

            return string.Format(@"select {0} from {1} {2}", selectStatementContent, tableName, whereClause);
        }
    }
}
