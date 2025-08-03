using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置源
/// </summary>
internal class DbConfigurationSource(DbConfigOptions options, ILoggerFactory? loggerFactory) : IConfigurationSource
{
    private readonly DbConfigOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly ILoggerFactory? _loggerFactory = loggerFactory;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DbConfigurationProvider(_options, _loggerFactory);
    }
}
