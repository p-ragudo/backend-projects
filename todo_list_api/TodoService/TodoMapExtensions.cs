namespace todo_list_api.TodoService;

public static class TodoMapExtensions
{
    public static TodoResponse ToResponse(this TodoItem item)
    {
        return new TodoResponse(
            item.Id,
            item.Title,
            item.IsCompleted,
            item.CreatedAtUtc
        );
    }

    public static TodoQueryResponse ToQueryResponse(this List<TodoItem> items)
    {
        return new TodoQueryResponse(
            items.Select(i => i.ToResponse())
                .ToList()
        );
    }

    public static TodoEditResponse ToEditResponse(this TodoItem item, TimeZoneInfo timeZone)
    {
        return new TodoEditResponse(
            item.Id,
            item.Title,
            item.IsCompleted,
            item.EditedAtUtc ?? TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone)
        );
    }

    public static TodoDeleteResponse ToDeleteResponse(this TodoItem item, TimeZoneInfo timeZone)
    {
        return new TodoDeleteResponse(
            item.ToResponse(),
            TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone)
        );
    }
}