namespace CompanyInfoApi.Services.Interfaces
{
    public interface IExcelProcessingService
    {
        Task<List<string>> ExtractColumnValuesAsync(IFormFile file, int columnIndex);
    }
}