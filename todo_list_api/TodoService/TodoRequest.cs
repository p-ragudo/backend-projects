namespace todo_list_api.TodoService;

public record TodoRequest (
    string Title
);

public record TodoQueryRequest(
    List<string>? Title,
    bool? IsCompleted
);

public record TodoEditRequest(
    string? Title,
    bool? IsCompleted
);

public record TodoDeleteRequest(
    int Id
);