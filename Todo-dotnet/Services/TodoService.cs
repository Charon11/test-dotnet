using Microsoft.EntityFrameworkCore;
using Todo_dotnet.Extensions;
using Todo_dotnet.Models;

namespace Todo_dotnet.Services;

public class TodoService : ITodoService
{
    private readonly TodoContext _context;

    public TodoService(TodoContext context)
    {
        _context = context;
    }

    public Task<TodoItem?> GetTodo(Guid id)
    {
        return _context.TodoItems.FindAsync(id).AsTask();
    }

    public Task<List<TodoItem>> GetTodos()
    {
        return _context.TodoItems.ToListAsync();
    }

    public Task<int> UpdateTodo(Guid id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return Task.FromException<int>(new BadHttpRequestException($"Given {id} is different from {todoItem.Id} Todo id"));
        }
        if (!TodoItemExists(id))
        {
            return Task.FromResult(0);
        }
        _context.Entry(todoItem).State = EntityState.Modified;
        return _context.SaveChangesAsync();
    }

    public Task<TodoItem> CreateTodo(TodoItem todoItem)
    {
        var entity = _context.TodoItems.Add(todoItem);
        return _context.SaveChangesAsync()
            .ContinueWith(_ => entity.Entity);
    }

    public Task<int> DeleteTodo(Guid id)
    {
        if (!TodoItemExists(id))
        {
            return Task.FromResult(0);
        }
        
        
        return _context.TodoItems.FindAsync(id).AsTask().FlatMap(r => {
            if (r == null)
            {
                return Task.FromResult(0);
            } 
            _context.TodoItems.Remove(r);
            return _context.SaveChangesAsync();
        });
    }


    
    private bool TodoItemExists(Guid id)
    {
        return _context.TodoItems.Any(e => e.Id == id);
    }
}