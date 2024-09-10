using CompanyInfoApi.Models;

public interface IGrainTradeService
{
    Task<CompanyInfo?> GetCompanyInfoByEdrpouAsync(string edrpou);
}
