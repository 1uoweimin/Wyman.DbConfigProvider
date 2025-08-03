using System.Data;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置选项
/// </summary>
internal class DbConfigOptions(Func<IDbConnection> dbConnection, string tableName, bool reloadOnChange, TimeSpan? reLoadInterval)
{
    public Func<IDbConnection> DbConnection { get; set; } = dbConnection;

    public string TableName { get; set; } = tableName;

    public bool ReloadOnChange { get; set; } = reloadOnChange;

    public TimeSpan? ReLoadInterval { get; set; } = reLoadInterval;
}
