using Microsoft.AspNetCore.Mvc;
using Todo_dotnet.Models;
using Todo_dotnet.Services;

namespace Todo_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodosController(ITodoService service)
        {
            _service = service;
        }

        // GET: api/Todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        { 
            return await _service.GetTodos();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(Guid id)
        {

            var todoItem = await _service.GetTodo(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/Todo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            return await _service.UpdateTodo(id, todoItem)
                .ContinueWith(r =>
                {
                     IActionResult res = r.Result == 0 ? NotFound() : NoContent();
                     return res;
                });
        }

        // POST: api/Todo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            return await _service.CreateTodo(todoItem)
                .ContinueWith(r => CreatedAtAction(nameof(GetTodoItem), new { id = r.Result.Id }, r.Result));
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            return await _service.DeleteTodo(id)
                .ContinueWith(r =>
                {
                    IActionResult res = r.Result == 0 ? NotFound() : NoContent();
                    return res;
                });
        }
    }
}
