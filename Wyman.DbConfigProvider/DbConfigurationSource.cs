using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置源
/// </summary>
internal class DbConfigurationSource : IConfigurationSource
{
    private readonly DbConfigOptions _options;
    private readonly ILoggerFactory? _loggerFactory;

    public DbConfigurationSource(DbConfigOptions options, ILoggerFactory? loggerFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _loggerFactory = loggerFactory;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DbConfigurationProvider(_options, _loggerFactory);
    }
}
