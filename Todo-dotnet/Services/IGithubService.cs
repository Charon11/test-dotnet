namespace Todo_dotnet.Services;

public interface IGithubService
{
    Task<string> getGHBranch();
}