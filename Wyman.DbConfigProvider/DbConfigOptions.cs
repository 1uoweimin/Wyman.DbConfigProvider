using System.Data;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置选项
/// </summary>
internal class DbConfigOptions
{
    public DbConfigOptions(Func<IDbConnection> dbConnection, DbType dbType, string tableName, bool reloadOnChange, TimeSpan? reLoadInterval)
    {
        DbConnection = dbConnection;
        DbType = dbType;
        TableName = tableName;
        ReloadOnChange = reloadOnChange;
        ReLoadInterval = reLoadInterval;
    }

    public Func<IDbConnection> DbConnection { get; set; }

    public DbType DbType { get; set; }

    public string TableName { get; set; }

    public bool ReloadOnChange { get; set; }

    public TimeSpan? ReLoadInterval { get; set; }
}
