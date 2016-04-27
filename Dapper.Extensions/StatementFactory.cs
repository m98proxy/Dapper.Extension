namespace Dapper
{
    public static class StatementFactory
    {
        public static string Select<TEntity>(object condition)
        {
            Kernel.SetDialect(Dialect.MSSQL);

            Kernel.CheckForKeyProperty<TEntity>();

            string selectStatementContent = Kernel.BuildSelectStatementContent<TEntity>();

            string tableName = Kernel.GetTableName<TEntity>();

            string whereClause = Kernel.BuildWhereClause<TEntity>(condition);

            return string.Format(@"select {0} from {1} {2}", selectStatementContent, tableName, whereClause);
        }
    }
}
