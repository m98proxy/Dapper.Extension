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

        public static string Insert<TEntity>()
        {
            Kernel.SetDialect(Dialect.MSSQL);

            Kernel.CheckForKeyProperty<TEntity>();

            string insertStatementContent = Kernel.BuildInsertStatementContent<TEntity>();

            string insertStatementValueContent = Kernel.BuildInsertStatementValueContent<TEntity>(false);

            string tableName = Kernel.GetTableName<TEntity>();

            return string.Format(@"insert into {0}({1}) values ({2})", tableName, insertStatementContent, insertStatementValueContent);
        }

        public static string Update<TEntity>()
        {
            Kernel.SetDialect(Dialect.MSSQL);

            Kernel.CheckForKeyProperty<TEntity>();

            string updateStatementContent = Kernel.BuildUpdateStatementContent<TEntity>();

            string tableName = Kernel.GetTableName<TEntity>();

            return string.Format(@"update {0} set {1}", tableName, updateStatementContent);
        }

        public static string Delete<TEntity>(object condition = null)
        {
            Kernel.SetDialect(Dialect.MSSQL);

            Kernel.CheckForKeyProperty<TEntity>();

            string tableName = Kernel.GetTableName<TEntity>();

            string whereClause = (condition != null) ? Kernel.BuildWhereClause<TEntity>(condition) : Kernel.BuildWhereKeyClause<TEntity>();

            return string.Format(@"delete from {0} {1}", tableName, whereClause);
        }
    }
}
