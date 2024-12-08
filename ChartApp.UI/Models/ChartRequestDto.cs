namespace ChartApp.UI.Models;

public class ChartRequestDto
{
    public string Server { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string TableName { get; set; }
    public string ColumnsRaw { get; set; }
    public List<string> Columns { get; set; }
    public string ChartType { get; set; } 
}