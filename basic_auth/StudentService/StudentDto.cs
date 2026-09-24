namespace basic_auth.StudentService;

public record CreateStudentRequest(
    string FirstName,
    string LastName
);

public record CreateStudentResponse(
    int Id,
    string FirstName,
    string LastName,
    bool IsEnrolled
);

public record GetStudentsQuery(
    List<int>? Id,
    List<string>? FirstName,
    List<string>? LastName,
    bool? IsEnrolled
);

public record GetStudentsResponse(
    List<Student> Students
);

public record UpdateStudentRequest(
    string? FirstName,
    string? LastName,
    bool? IsEnrolled
);

public record UpdateStudentResponse(
    int Id,
    string FirstName,
    string LastName,
    bool IsEnrolled
);

public record DeleteStudentResponse(
    int Id,
    string FirstName,
    string LastName,
    bool IsEnrolled
);