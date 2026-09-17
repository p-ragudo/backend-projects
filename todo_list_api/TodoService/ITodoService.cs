namespace todo_list_api.TodoService;

public interface ITodoService
{
    Task<TodoResponse> Create(TodoRequest request);
    Task<TodoQueryResponse> Get(TodoQueryRequest? request);
    Task<TodoResponse?> GetById(int id);
    Task<TodoEditResponse?> UpdateById(int id, TodoEditRequest request);
    Task<TodoDeleteResponse?> DeleteById(int id);
}