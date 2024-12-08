using ChartApp.Api.Models;
using ChartApp.Api.Services.Abstract;
using Microsoft.Data.SqlClient;

namespace ChartApp.Api.Services.Concrete;

public class DataService : IDataService
{
    private string BuildConnectionString(DbConnectionDto connectionDto)
    {
        return
            $"Server={connectionDto.Server};Database={connectionDto.Database};User Id={connectionDto.Username};Password={connectionDto.Password};TrustServerCertificate=True;";
    }

    public async Task<bool> TestConnectionAsync(DbConnectionDto connectionDto)
    {
        var connectionString = BuildConnectionString(connectionDto);
        await using var connection = new SqlConnection(connectionString);
        try
        {
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetTablesAsync(DbConnectionDto connectionDto)
    {
        var connectionString = BuildConnectionString(connectionDto);
        const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
        return await ExecuteQueryAsync(connectionString, query);
    }

    public async Task<List<string>> GetColumnsAsync(DbConnectionDto connectionDto, string tableName)
    {
        var connectionString = BuildConnectionString(connectionDto);
        var query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";

        return await ExecuteQueryAsync(connectionString, query, new SqlParameter("@TableName", tableName));
    }
    
    //get chart data with table and columns
    public async Task<List<string>> GetChartDataAsync(DbConnectionDto connectionDto, string tableName,
        List<string> columns)
    {
        var connectionString = BuildConnectionString(connectionDto);
        var query = $"SELECT {string.Join(", ", columns)} FROM {tableName}";

        return await ExecuteQueryAsync(connectionString, query);
    }
    
    public async Task<IEnumerable<Dictionary<string, object>>> GetChartDataAsync(DbConnectionDto connectionDto, string tableName, string[] columnNames)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Tablo adı belirtilmelidir.", nameof(tableName));

        if (columnNames == null || !columnNames.Any())
            throw new ArgumentException("En az bir sütun adı belirtilmelidir.", nameof(columnNames));

        var selectedColumns = string.Join(", ", columnNames.Select(c => $"[{c}]"));
        var query = $"SELECT {selectedColumns} FROM [{tableName}]";

        var connectionString = BuildConnectionString(connectionDto);

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                var result = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    foreach (var column in columnNames)
                    {
                        row[column] = reader[column];
                    }
                    result.Add(row);
                }

                return result;
            }
        }
    }


    
    private async Task<List<string>> ExecuteQueryAsync(string connectionString, string query,
        params SqlParameter[] parameters)
    {
        var results = new List<string>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        
        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }
    
}