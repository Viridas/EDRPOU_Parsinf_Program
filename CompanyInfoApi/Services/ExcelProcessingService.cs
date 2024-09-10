using ClosedXML.Excel;
using CompanyInfoApi.Services.Interfaces;

public class ExcelProcessingService : IExcelProcessingService
{
    public async Task<List<string>> ExtractColumnValuesAsync(IFormFile file, int columnIndex)
    {
        var values = new List<string>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();

                if (worksheet != null)
                {
                    foreach (var row in worksheet.RowsUsed())
                    {
                        var cellValue = row.Cell(columnIndex + 1).GetValue<string>().Trim();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            values.Add(cellValue);
                        }
                    }
                }
            }
        }

        return values;
    }
}
