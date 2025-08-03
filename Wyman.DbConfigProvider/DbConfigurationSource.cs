using Microsoft.Extensions.Configuration;
namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置源
/// </summary>
internal class DbConfigurationSource(DbConfigOptions options) : IConfigurationSource
{
    private readonly DbConfigOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DbConfigurationProvider(_options);
    }
}
