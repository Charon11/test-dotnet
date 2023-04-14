namespace Todo_dotnet.Services;

public interface IGithubService
{
    Task<IList<dynamic>> GetGhBranch();
}