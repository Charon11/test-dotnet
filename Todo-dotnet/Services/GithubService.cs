using Microsoft.Net.Http.Headers;
using Flurl.Http;

namespace Todo_dotnet.Services;

public class GithubService : IGithubService
{
    
    public Task<IList<dynamic>> GetGhBranch()
    {
        
        return "https://api.github.com/repos/Charon11/sfeir-pokedex/branches"
            .WithHeader(HeaderNames.Accept, "application/vnd.github.v3+json")
            .WithHeader(HeaderNames.UserAgent, "HttpRequestsSample")
            .GetJsonListAsync();
        
    }
}