namespace ChartApp.Api.Models;

public class ChartDataRequestDto
{
    public DbConnectionDto ConnectionDto { get; set; }
    public List<string> Columns { get; set; }
    public string TableName { get; set; }
}