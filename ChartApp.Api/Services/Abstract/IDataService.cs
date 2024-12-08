using ChartApp.Api.Models;

namespace ChartApp.Api.Services.Abstract;

public interface IDataService
{
    Task<bool> TestConnectionAsync(DbConnectionDto connectionDto);
    Task<List<string>> GetTablesAsync(DbConnectionDto connectionDto);
    Task<List<string>> GetColumnsAsync(DbConnectionDto connectionDto, string tableName);
    //get chart data with table and columns
    Task<IEnumerable<Dictionary<string, object>>> GetChartDataAsync(DbConnectionDto connectionDto, string tableName,
        string[] columnNames);
}