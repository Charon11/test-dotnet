namespace Todo_dotnet.Extensions;

public static class TaskExtensions
{
    public static async Task<TResult> FlatMap<T, TResult>(this Task<T> task, Func<T, Task<TResult>> fn)
    {
        var value = await task;
    
        return await fn(value);
    }
}