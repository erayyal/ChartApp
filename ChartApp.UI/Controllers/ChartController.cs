using ChartApp.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChartApp.UI.Controllers;

public class ChartController(HttpClient httpClient) : Controller
{
    [HttpGet]
    public IActionResult ChartResult()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChartResult(ChartRequestDto request)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(request.ColumnsRaw))
            {
                request.Columns = request.ColumnsRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .ToList();
            }

            var baseUrl = "https://localhost:7151/api/Data/GetChartData";
            var columnParams = string.Join("&columnNames=", request.Columns);
            var url = $"{baseUrl}?tableName={request.TableName}&columnNames={columnParams}";

            var connectionDto = new DbConnectionDto
            {
                Server = request.Server,
                Database = request.Database,
                Username = request.Username,
                Password = request.Password
            };

            var response = await httpClient.PostAsJsonAsync(url, connectionDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Veri alınamadı. Hata detayı: {errorContent}";
                return View();
            }

            var chartData = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
            ViewBag.ChartData = chartData;
            ViewBag.ChartType = request.ChartType;

            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
            return View();
        }
    }
}