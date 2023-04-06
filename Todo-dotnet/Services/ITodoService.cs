using Todo_dotnet.Models;

namespace Todo_dotnet.Services;

public interface ITodoService
{
    Task<TodoItem?> GetTodo(Guid id);
    Task<List<TodoItem>> GetTodos();

    Task<int> UpdateTodo(Guid id, TodoItem todoItem);

    Task<TodoItem> CreateTodo(TodoItem todoItem);

    Task<int> DeleteTodo(Guid id);

}