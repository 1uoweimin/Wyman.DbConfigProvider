using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Npgsql;
using Microsoft.Data.SqlClient;
using Test_Console_8;
using Test_Console_8.Controllers;
using Wyman.DbConfigProvider;

ServiceCollection sc = new ServiceCollection();

sc.AddScoped<TestConfigsController>();

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

var dbType = DbType.MySql;
switch (dbType)
{
    case DbType.MySql:
        var connStr1 = "Server=127.0.0.1;database=3 dbconfig;uid=root;pwd=123456;";
        configurationBuilder.AddDbConfiguration(() => new MySqlConnection(connStr1), DbType.MySql, "mysql_configs");
        break;
    case DbType.SQLServer:
        var connStr2 = "Server=DEVICEOFLWM;Database=3 dbconfig;User ID=sa;Password=123456;TrustServerCertificate=true;";
        configurationBuilder.AddDbConfiguration(() => new SqlConnection(connStr2), DbType.SQLServer, "sqlserver_configs");
        break;
    case DbType.PostgreSQL:
        var connStr3 = "Server=127.0.0.1;Port=5432;Database=3 dbconfig;User ID=postgres;Password=123456;";
        configurationBuilder.AddDbConfiguration(() => new NpgsqlConnection(connStr3), DbType.PostgreSQL, "pg_configs");
        break;
}

IConfigurationRoot configRoot = configurationBuilder.Build();

sc.AddOptions()
    .Configure<Ftp>(e => configRoot.GetSection("Ftp").Bind(e))
    .Configure<Cors>(e => configRoot.GetSection("Cors").Bind(e));

using ServiceProvider sp = sc.BuildServiceProvider();
var testConfigsController = sp.GetRequiredService<TestConfigsController>();
testConfigsController.GetConfigs();