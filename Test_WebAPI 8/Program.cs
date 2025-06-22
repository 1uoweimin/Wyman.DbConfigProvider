using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Test_WebAPI_8;
using Wyman.DbConfigProvider;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

var dbType = DbType.SQLServer;

switch (dbType)
{
    case DbType.MySql:
        var connMysql = builder.Configuration.GetConnectionString("connMysql");
        builder.Configuration.AddDbConfiguration(
            () => new MySqlConnection(connMysql),
            DbType.MySql,
            tableName: "mysql_configs",
            reloadOnChange: true,
            loggerFactory: loggerFactory
        );
        break;
    case DbType.SQLServer:
        var connSqlServer = builder.Configuration.GetConnectionString("connSqlServer");
        builder.Configuration.AddDbConfiguration(
            () => new SqlConnection(connSqlServer),
            DbType.SQLServer,
            tableName: "sqlserver_configs",
            reloadOnChange: true,
            loggerFactory: loggerFactory
        );
        break;
    case DbType.PostgreSQL:
        var postgresqlServer = builder.Configuration.GetConnectionString("connPostgresql");
        builder.Configuration.AddDbConfiguration(
            () => new NpgsqlConnection(postgresqlServer),
            DbType.PostgreSQL,
            tableName: "pg_configs",
            reloadOnChange: true,
            loggerFactory: loggerFactory
        );
        break;
}

services.AddOptions()
    .Configure<Ftp>(builder.Configuration.GetSection("Ftp"))
    .Configure<Cors>(builder.Configuration.GetSection("Cors"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
