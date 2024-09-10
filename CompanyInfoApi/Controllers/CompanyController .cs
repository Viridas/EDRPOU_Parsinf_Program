using ClosedXML.Excel;
using CompanyInfoApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class CompanyController : ControllerBase
{
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly GrainTradeService _grainTradeService;
    private readonly ExcelProcessingService _excelProcessingService;
    public CompanyController(GrainTradeService grainTradeService, ExcelProcessingService excelProcessingService)
    {
        _grainTradeService = grainTradeService;
        _excelProcessingService = excelProcessingService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCompanyFile([FromForm] IFormFile file, [FromForm] int collumIndex)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
        {
            return BadRequest("Only .xlsx files are allowed.");
        }

        var companyEDRPOU = await _excelProcessingService.ExtractColumnValuesAsync(file, collumIndex);
        List<CompanyInfo> companyData = new List<CompanyInfo>();

        foreach (var company in companyEDRPOU)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var companyInfo = await _grainTradeService.GetCompanyInfoByEdrpouAsync(company);
                if (companyInfo != null && !companyData.Any(c => c.EDRPOUCode == companyInfo.EDRPOUCode))
                {
                    companyData.Add(companyInfo);
                    Console.WriteLine("Company Added");
                }
            }
            catch (Exception ex)
            {
                break;
            }

        }

        var resultFile = GenerateExcelFile(companyData);

        var memoryStream = new MemoryStream(resultFile);
        memoryStream.Position = 0;

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CompanyDataResult.xlsx");
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopProgram([FromBody] bool IsProgramWorking)
    {
        if (IsProgramWorking == false)
        {
            _cancellationTokenSource.Cancel();
            return Ok(new { message = "Upload stopped" });
        }
        return BadRequest(new { message = "Invalid stop flag value" });
    }


    private byte[] GenerateExcelFile(List<CompanyInfo> companyData)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Company Info");

            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Phone";
            worksheet.Cell(1, 3).Value = "EDRPOU Code";

            for (int i = 0; i < companyData.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = companyData[i].Name;
                worksheet.Cell(i + 2, 2).Value = companyData[i].Phone;
                worksheet.Cell(i + 2, 3).Value = companyData[i].EDRPOUCode;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}
