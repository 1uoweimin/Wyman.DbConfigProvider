using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="tableName"></param>
    /// <param name="reloadOnChange"></param>
    /// <param name="reloadInterval"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddDbConfiguration(this ConfigurationManager builder, Func<IDbConnection> dbConnection,
        string tableName = "_Configs", bool reloadOnChange = false, TimeSpan? reloadInterval = null)
    {
        var dbConfigOption = new DbConfigOptions(dbConnection, tableName, reloadOnChange, reloadInterval);
        // 插入数据库配置源到配置构建器的最前面
        builder.Sources.Insert(0, new DbConfigurationSource(dbConfigOption));
        return builder;
    }

    public static IApplicationBuilder UseDbConfiguration(this IApplicationBuilder builder)
    {
        // 设置数据库配置提供者的全局日志工厂
        if (builder.ApplicationServices.GetService<ILoggerFactory>() is ILoggerFactory loggerFactory)
        {
            DbConfigurationProvider.GlobalLoggerFactory = loggerFactory;
        }
        return builder;
    }
}
