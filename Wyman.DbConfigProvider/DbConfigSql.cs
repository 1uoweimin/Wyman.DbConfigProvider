namespace Wyman.DbConfigProvider;

internal class DbConfigSql(string tableName)
{
    public string Id => "cid";
    public string Key => "ckey";
    public string Value => "cvalue";
    public virtual string GetTableSql => $"SELECT {Key},{Value} FROM {tableName} WHERE {Id} IN(SELECT MAX({Id}) FROM {tableName} GROUP BY {Key});";
}

