using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置提供者扩展方法
/// </summary>
public static class DbConfigurationProviderExtensions
{
    /// <summary>
    /// 添加数据库配置源
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbType"></param>
    /// <param name="tableName"></param>
    /// <param name="reloadOnChange"></param>
    /// <param name="reloadInterval"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder, Func<IDbConnection> dbConnection,
        DbType dbType = DbType.Other, string tableName = "_Configs", bool reloadOnChange = false, TimeSpan? reloadInterval = null, ILoggerFactory? loggerFactory = null)
    {
        var dbConfigOption = new DbConfigOptions(dbConnection, dbType, tableName, reloadOnChange, reloadInterval);
        return builder.Add(new DbConfigurationSource(dbConfigOption, loggerFactory));
    }
}
