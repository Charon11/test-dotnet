using Microsoft.Net.Http.Headers;
using NuGet.Protocol;

namespace Todo_dotnet.Services;

public class GithubService : IGithubService
{
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    
    public GithubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
    }
    
    public Task<string> getGHBranch()
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.github.com/repos/dotnet/AspNetCore.Docs/branches")
        {
            Headers =
            {
                { HeaderNames.Accept, "application/vnd.github.v3+json" },
                { HeaderNames.UserAgent, "HttpRequestsSample" }
            }
        };
        return _httpClient
            .SendAsync(httpRequestMessage)
            .ContinueWith(r => r.Result.IsSuccessStatusCode ? new StreamReader(r.Result.Content.ReadAsStream()).ReadToEnd() :"nothing");
    }
}