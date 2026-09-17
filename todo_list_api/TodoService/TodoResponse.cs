namespace todo_list_api.TodoService;

public record TodoResponse (
    int Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt
);

public record TodoQueryResponse(
    List<TodoResponse> Items
);

public record TodoEditResponse(
    int Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset EditedAt
);

public record TodoDeleteResponse(
    TodoResponse Item,
    DateTimeOffset DeletedAt
);