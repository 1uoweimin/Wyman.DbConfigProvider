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

var dbType = DbType.MySql;

switch (dbType)
{
    case DbType.MySql:
        var connMysql = builder.Configuration.GetConnectionString("connMysql");
        DbConfigInitialization.CreateTable("mysql_configs", new MySqlConnection(connMysql), DbType.MySql);
        builder.Configuration.AddDbConfiguration(
            () => new MySqlConnection(connMysql),
            tableName: "mysql_configs",
            reloadOnChange: true,
            loggerFactory: loggerFactory
        );
        break;
    case DbType.SQLServer:
        var connSqlServer = builder.Configuration.GetConnectionString("connSqlServer");
        DbConfigInitialization.CreateTable("sqlserver_configs", new MySqlConnection(connSqlServer), DbType.SQLServer);
        builder.Configuration.AddDbConfiguration(
            () => new SqlConnection(connSqlServer),
            tableName: "sqlserver_configs",
            reloadOnChange: true,
            loggerFactory: loggerFactory
        );
        break;
    case DbType.PostgreSQL:
        var postgresqlServer = builder.Configuration.GetConnectionString("connPostgresql");
        DbConfigInitialization.CreateTable("pg_configs", new MySqlConnection(postgresqlServer), DbType.PostgreSQL);
        builder.Configuration.AddDbConfiguration(
            () => new NpgsqlConnection(postgresqlServer),
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
