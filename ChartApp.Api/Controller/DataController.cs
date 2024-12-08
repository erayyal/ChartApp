using ChartApp.Api.Models;
using ChartApp.Api.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ChartApp.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class DataController(IDataService dataService) : ControllerBase
{
    [HttpPost("TestConnection")]
    public async Task<IActionResult> TestConnection([FromBody] DbConnectionDto connectionDto)
    {
        try
        {
            if (await dataService.TestConnectionAsync(connectionDto))
            {
                return Ok(new { message = "Bağlantı başarılı." });
            }

            return BadRequest(new { message = "Bağlantı bilgileri geçersiz." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Bağlantı sağlanamadı.", error = ex.Message });
        }
    }

    [HttpPost("GetTables")]
    public async Task<IActionResult> GetTables([FromBody] DbConnectionDto connectionDto)
    {
        try
        {
            var tables = await dataService.GetTablesAsync(connectionDto);
            return Ok(tables);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Tablo bilgileri alınamadı.", error = ex.Message });
        }
    }

    [HttpPost("GetColumns")]
    public async Task<IActionResult> GetColumns([FromBody] DbConnectionDto connectionDto, [FromQuery] string tableName)
    {
        try
        {
            var columns = await dataService.GetColumnsAsync(connectionDto, tableName);
            return Ok(columns);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Kolon bilgileri alınamadı.", error = ex.Message });
        }
    }
    
    
    [HttpPost("GetChartData")]
    public async Task<IActionResult> GetChartData([FromBody] DbConnectionDto connectionDto, [FromQuery] string tableName, [FromQuery] string[] columnNames)
    {
        try
        {
            if (columnNames == null || !columnNames.Any())
            {
                return BadRequest(new { message = "En az bir sütun adı belirtilmelidir." });
            }

            var data = await dataService.GetChartDataAsync(connectionDto, tableName, columnNames);

            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Veri alınırken bir hata oluştu.", error = ex.Message });
        }
    }

}