# Wyman.DbConfigProvider
一个.NET配置提供程序，支持从多种数据库（SQL Server/MySQL/PostgreSQL/Oracle/SQLite等）加载应用配置。

## 数据库要求
创建配置表（默认名_configs，可自定义）</br>
必需字段：</br>

    cid：整数，自增列（主键）</br>
    ckey：字符串，配置项名称</br>
    cvalue：字符串，配置项值</br>
    
注意：可以通过在程序中指定DbType自动生成配置表，DbType中不存在只能手动创建配置表。</br>

## 配置规则
键名格式：支持多级配置（如RedisConnStr、Ftp:IP）</br>
值格式：</br>

    普通值：localhost</br>
    JSON对象：{"IP":"127.0.0.1","UserName":"root"}</br>
    JSON数组：["Content-Type","ApiToken"]</br>
    注意：必须使用双引号的标准JSON格式</br>
    
版本控制：相同ckey时，取cid最大的记录生效</br>

## 使用方法
参考Demo项目：</br>
控制台应用：查看Test_Console示例</br>
WebAPI项目：查看Test_WebAPI示例</br>

