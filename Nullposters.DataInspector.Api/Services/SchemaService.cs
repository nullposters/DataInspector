using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Nullposters.DataInspector.Api.Services
{
    public class SchemaService
    {
        public async Task<List<TableSchema>> GetTableSchemasAsync(string connectionString, string provider)
        {
            using var context = new SchemaContext(connectionString, provider.Trim());

            string tableQuery = GetTableQueryByProvider(provider);

            var tables = await context.Database.SqlQueryRaw<string>(tableQuery).ToListAsync();

            var tableSchemas = new List<TableSchema>();
            foreach (var table in tables)
            {
                var columns = await context.Database.SqlQueryRaw<ColumnSchema>(
                    GetColumnQueryByProvider(provider),
                    new SqlParameter("@TableName", table)
                ).ToListAsync();

                tableSchemas.Add(new TableSchema
                {
                    TableName = table,
                    Columns = columns
                });
            }

            return tableSchemas;
        }

        private static string GetTableQueryByProvider(string provider)
        {
            return provider switch
            {
                "SqlServer" => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                "PostgreSQL" => "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'",
                "MySQL" => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                _ => throw new NotSupportedException($"The provider {provider} is not supported."),
            };
        }

        private static string GetColumnQueryByProvider(string provider)
        {
            switch (provider)
            {
                case "SqlServer":
                    return @"
                    SELECT 
                        COLUMN_NAME AS ColumnName, 
                        DATA_TYPE AS DataType, 
                        CASE 
                            WHEN DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar') THEN CHARACTER_MAXIMUM_LENGTH
                            ELSE NULL
                        END AS CharacterMaximumLength, 
                        IS_NULLABLE AS IsNullable, 
                        COLUMN_DEFAULT AS ColumnDefault,
                        CAST(
                            (SELECT CASE WHEN EXISTS (
                                SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE K 
                                WHERE K.TABLE_NAME = C.TABLE_NAME AND K.COLUMN_NAME = C.COLUMN_NAME AND K.CONSTRAINT_NAME LIKE 'PK%')
                                THEN 1 ELSE 0 END) AS BIT) AS IsPrimaryKey,
                        CAST(
                            (SELECT CASE WHEN EXISTS (
                                SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE K 
                                WHERE K.TABLE_NAME = C.TABLE_NAME AND K.COLUMN_NAME = C.COLUMN_NAME AND K.CONSTRAINT_NAME LIKE 'FK%')
                                THEN 1 ELSE 0 END) AS BIT) AS IsForeignKey
                    FROM 
                        INFORMATION_SCHEMA.COLUMNS C
                    WHERE 
                        TABLE_NAME = @TableName;";

                case "PostgreSQL":
                    return @"
                    SELECT 
                        column_name AS ColumnName, 
                        data_type AS DataType, 
                        CASE 
                            WHEN data_type IN ('character varying', 'varchar', 'char') THEN character_maximum_length
                            ELSE NULL
                        END AS CharacterMaximumLength, 
                        is_nullable AS IsNullable, 
                        column_default AS ColumnDefault,
                        EXISTS (
                            SELECT 1 FROM information_schema.table_constraints T
                            JOIN information_schema.key_column_usage K 
                            ON K.constraint_name = T.constraint_name 
                            WHERE K.column_name = C.column_name AND K.table_name = C.table_name AND T.constraint_type = 'PRIMARY KEY'
                        ) AS IsPrimaryKey,  -- Returns true/false
                        EXISTS (
                            SELECT 1 FROM information_schema.table_constraints T
                            JOIN information_schema.key_column_usage K 
                            ON K.constraint_name = T.constraint_name 
                            WHERE K.column_name = C.column_name AND K.table_name = C.table_name AND T.constraint_type = 'FOREIGN KEY'
                        ) AS IsForeignKey  -- Returns true/false
                    FROM 
                        information_schema.columns C
                    WHERE 
                        table_name = @TableName;";

                case "MySQL":
                    return @"
                    SELECT 
                        COLUMN_NAME AS ColumnName, 
                        DATA_TYPE AS DataType, 
                        CASE 
                            WHEN DATA_TYPE IN ('varchar', 'char') THEN CHARACTER_MAXIMUM_LENGTH
                            ELSE NULL
                        END AS CharacterMaximumLength, 
                        IS_NULLABLE AS IsNullable, 
                        COLUMN_DEFAULT AS ColumnDefault,
                        CAST((COLUMN_KEY = 'PRI') AS BOOLEAN) AS IsPrimaryKey,
                        CAST((COLUMN_KEY = 'MUL') AS BOOLEAN) AS IsForeignKey
                    FROM 
                        INFORMATION_SCHEMA.COLUMNS
                    WHERE 
                        TABLE_NAME = @TableName;";

                default:
                    throw new NotSupportedException($"The provider {provider} is not supported.");
            }
        }
    }
}
