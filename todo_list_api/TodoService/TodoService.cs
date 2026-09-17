using Microsoft.EntityFrameworkCore;

namespace todo_list_api.TodoService;

public class TodoService : ITodoService
{
    private readonly DbContext _context;
    private readonly TimeZoneInfo TargetTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

    public TodoService(DbContext context)
    {
        _context = context;
    }

    public async Task<TodoResponse> Create(TodoRequest request)
    {
        var newItem = new TodoItem
        {
            Title = request.Title,
            IsCompleted = false,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _context.TodoItems.Add(newItem);
        await _context.SaveChangesAsync();

        return newItem.ToResponse();
    }


    public async Task<TodoQueryResponse> Get(TodoQueryRequest? request)
    {
        List<TodoItem> items;

        if (request == null)
        {
            items = await _context.TodoItems
                .AsNoTracking()
                .ToListAsync();

            return items.ToQueryResponse();
        }

        var query = _context.TodoItems
            .AsNoTracking()
            .AsQueryable();

        if (request?.Title is { Count: > 0})
        {
            var terms = request.Title
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .ToList();
            
            if (terms.Count > 0)
            {
                query = query.Where(q => terms.Any(t => q.Title.Contains(t)));
            }
        }

        if (request?.IsCompleted != null)
        {
            query = query.Where(q => q.IsCompleted == request.IsCompleted);
        }

        items = await query
            .Select(q => new TodoItem
            {
                Id = q.Id,
                Title = q.Title,
                IsCompleted = q.IsCompleted
            })
            .ToListAsync();
        
        return items.ToQueryResponse();
    }

    public async Task<TodoResponse?> GetById(int id)
    {
        var item = _context.TodoItems
            .FirstOrDefault(item => item.Id == id);

        if (item == null)
        {
            return null;
        }

        return item.ToResponse();
    }

    public async Task<TodoEditResponse?> UpdateById(
        int id,
        TodoEditRequest request
    )
    {
        if (request.Title == null && request.IsCompleted == null)
        {
            return null;    
        }

        var item = _context.TodoItems
            .FirstOrDefault(item => item.Id == id);
        
        if (item == null)
        {
            return null;
        }
        
        item.Title = request.Title ?? item.Title;
        item.IsCompleted = request.IsCompleted ?? item.IsCompleted;
        item.EditedAtUtc = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return item.ToEditResponse(TargetTimeZone);
    }

    public async Task<TodoDeleteResponse?> DeleteById(int id)
    {
        var item = await _context.TodoItems.FirstOrDefaultAsync(item => item.Id == id);

        if (item == null)
        {
            return null;
        }

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();

        return item.ToDeleteResponse(TargetTimeZone);
    }        
}