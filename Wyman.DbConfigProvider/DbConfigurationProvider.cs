using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Wyman.DbConfigProvider;

/// <summary>
/// 数据库配置提供者
/// </summary>
internal class DbConfigurationProvider : ConfigurationProvider, IDisposable
{
    public static ILoggerFactory? GlobalLoggerFactory { get; set; }
    private ILogger<DbConfigurationProvider>? _logger => GlobalLoggerFactory?.CreateLogger<DbConfigurationProvider>();

    private readonly DbConfigOptions _options;
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task? _reloadTask;
    private bool _disposed = false;

    public DbConfigurationProvider(DbConfigOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (options.ReloadOnChange)
        {
            _reloadTask = Task.Run(ReloadLoopAsync);
        }
    }

    public override void Load()
    {
        try
        {
            _lock.EnterWriteLock();
            var oldData = new Dictionary<string, string?>(Data);
            Data.Clear();
            LoadConfigurationData();
            if (HasConfigurationChanged(oldData, Data))
            {
                _logger?.LogInformation("Configuration changed, triggering reload");
                OnReload();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error loading configuration from database");
            throw;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _cts.Cancel();
            _reloadTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error waiting for reload task to complete");
        }
        finally
        {
            _cts.Dispose();
            _lock.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    #region Private Methods

    private async Task ReloadLoopAsync()
    {
        var interval = _options.ReLoadInterval ?? TimeSpan.FromSeconds(5);

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, _cts.Token);
                Load();
            }
            catch (OperationCanceledException)
            {
                // 正常取消，退出循环
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in configuration reload loop");
                await Task.Delay(TimeSpan.FromSeconds(60), _cts.Token);
            }
        }
    }

    private void LoadConfigurationData()
    {
        using var connection = _options.DbConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = new DbConfigSql(_options.TableName).GetTableSql;
        command.CommandTimeout = 30;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            try
            {
                var key = reader.GetString(0);
                var value = reader.GetString(1);

                if (string.IsNullOrEmpty(key))
                {
                    _logger?.LogWarning("Skipping configuration entry with null or empty key");
                    continue;
                }

                ProcessConfigurationValue(key, value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing configuration row");
            }
        }
    }

    private void ProcessConfigurationValue(string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Data[key] = value;
            return;
        }

        var trimmedValue = value.Trim();
        if (IsJsonValue(trimmedValue))
        {
            TryParseJsonValue(key, trimmedValue);
        }
        else
        {
            Data[key] = trimmedValue;
        }
    }

    private void TryParseJsonValue(string key, string value)
    {
        try
        {
            var jsonOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            using var document = JsonDocument.Parse(value, jsonOptions);
            ParseJsonElement(key, document.RootElement);
        }
        catch (JsonException ex)
        {
            _logger?.LogWarning(ex, "Failed to parse value as JSON for key '{Key}', treating as plain text", key);
            Data[key] = value;
        }
    }

    private void ParseJsonElement(string key, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var childKey = $"{key}{ConfigurationPath.KeyDelimiter}{property.Name}";
                    ParseJsonElement(childKey, property.Value);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var childKey = $"{key}{ConfigurationPath.KeyDelimiter}{index}";
                    ParseJsonElement(childKey, item);
                    index++;
                }
                break;

            case JsonValueKind.String:
                Data[key] = element.GetString();
                break;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                Data[key] = null;
                break;

            default:
                Data[key] = element.GetRawText();
                break;
        }
    }

    private bool IsJsonValue(string value)
    {
        return (value.StartsWith("{") && value.EndsWith("}")) ||
               (value.StartsWith("[") && value.EndsWith("]"));
    }

    private static bool HasConfigurationChanged(IDictionary<string, string?> oldData, IDictionary<string, string?> newData)
    {
        if (oldData.Count != newData.Count) return true;

        foreach (var kvp in oldData)
        {
            if (!newData.TryGetValue(kvp.Key, out var newValue) ||
                !string.Equals(kvp.Value, newValue, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
