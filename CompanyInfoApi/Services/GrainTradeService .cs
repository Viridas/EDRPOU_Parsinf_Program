using CompanyInfoApi.Models;
using HtmlAgilityPack;

public class GrainTradeService : IGrainTradeService
{
    private readonly HttpClient _httpClient;

    public GrainTradeService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<CompanyInfo?> GetCompanyInfoByEdrpouAsync(string edrpou)
    {
        var searchUrl = $"https://graintrade.com.ua/traideri?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl1 = $"https://graintrade.com.ua/proizvoditeli?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl2 = $"https://graintrade.com.ua/elevatori?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl3 = $"https://graintrade.com.ua/ekspeditori?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl4 = $"https://graintrade.com.ua/porti?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl5 = $"https://graintrade.com.ua/terminali?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl6 = $"https://graintrade.com.ua/holdingi?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";
        var searchUrl7 = $"https://graintrade.com.ua/pererabotchiki?re=1&ajax=list-view-users&regions=&areas=&searchInput={edrpou}";

        var companies = await GetCompanys(searchUrl);

        if (companies != null)
        {
            CompanyInfo companyInfo = await GenerateAnswer(companies, edrpou);
            return companyInfo;
        }
        else
        {
            var companies1 = await GetCompanys(searchUrl1);
            if (companies1 != null)
            {
                CompanyInfo companyInfo = await GenerateAnswer(companies1, edrpou);
                return companyInfo;
            }
            else
            {
                var companies2 = await GetCompanys(searchUrl2);
                if (companies2 != null)
                {
                    CompanyInfo companyInfo = await GenerateAnswer(companies2, edrpou);
                    return companyInfo;
                }
                else
                {
                    var companies3 = await GetCompanys(searchUrl3);
                    if (companies3 != null)
                    {
                        CompanyInfo companyInfo = await GenerateAnswer(companies3, edrpou);
                        return companyInfo;
                    }
                    else
                    {
                        var companies4 = await GetCompanys(searchUrl4);
                        if (companies4 != null)
                        {
                            CompanyInfo companyInfo = await GenerateAnswer(companies4, edrpou);
                            return companyInfo;
                        }
                        else
                        {
                            var companies5 = await GetCompanys(searchUrl5);
                            if (companies5 != null)
                            {
                                CompanyInfo companyInfo = await GenerateAnswer(companies5, edrpou);
                                return companyInfo;
                            }
                            else
                            {
                                var companies6 = await GetCompanys(searchUrl6);
                                if (companies6 != null)
                                {
                                    CompanyInfo companyInfo = await GenerateAnswer(companies6, edrpou);
                                    return companyInfo;
                                }
                                else
                                {
                                    var companies7 = await GetCompanys(searchUrl7);
                                    if (companies7 != null)
                                    {
                                        CompanyInfo companyInfo = await GenerateAnswer(companies7, edrpou);
                                        return companyInfo;
                                    }
                                    else return new CompanyInfo { Name = "Не знайдено", Phone = "Не знайдено", EDRPOUCode = edrpou };
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    private async Task<CompanyInfo> GenerateAnswer(HtmlNodeCollection companies, string edrpou)
    {
        var companyLinks = new List<CompanyInfo>();
        foreach (var company in companies)
        {
            var companyName = company.InnerText.Trim();

            var companyUrl = "https://graintrade.com.ua" + company.GetAttributeValue("href", string.Empty);

            var companyPhone = await GetCompanyPhoneNumberAsync(companyUrl);

            companyLinks.Add(new CompanyInfo
            {
                Name = companyName,
                Phone = companyPhone,
                EDRPOUCode = edrpou
            });

        }
        return companyLinks.FirstOrDefault();
    }
    private async Task<HtmlNodeCollection> GetCompanys(string searchUrl)
    {
        var response = await _httpClient.GetAsync(searchUrl);
        response.EnsureSuccessStatusCode();

        var pageContents = await response.Content.ReadAsStringAsync();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(pageContents);

        return htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'companyTitle')]/a");
    }

    private async Task<string> GetCompanyPhoneNumberAsync(string companyUrl)
    {
        var companyResponse = await _httpClient.GetAsync(companyUrl);

        companyResponse.EnsureSuccessStatusCode();

        var companyPageContents = await companyResponse.Content.ReadAsStringAsync();

        var htmlDoc = new HtmlDocument();

        htmlDoc.LoadHtml(companyPageContents);

        var phoneNumber = GetPhoneNumberFromHtml(htmlDoc);

        return phoneNumber ?? "Номер телефону не знайдено";
    }

    private string GetPhoneNumberFromHtml(HtmlDocument htmlDoc)
    {
        var div1 = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]");

        if (div1 != null)
        {
            var div2 = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]");
            if (div2 != null)
            {
                var div3 = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]/div[1]/div[6]");
                if (div3 != null)
                {
                    var tab = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]/div[1]/div[6]/div[1]/table");
                    if (tab != null)
                    {
                        var tr = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]/div[1]/div[6]/div[1]/table/tr[4]");
                        if (tr != null)
                        {
                            var end = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]/div[1]/div[6]/div[1]/table/tr[4]/td[2]/label/b");
                            if (end != null)
                                return end.InnerText.Trim();

                        }
                    }

                }
            }
        }

        return null;
    }


}

