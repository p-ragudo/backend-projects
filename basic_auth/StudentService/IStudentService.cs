namespace basic_auth.StudentService;

public interface IStudentService
{
    Task<CreateStudentResponse> Create(CreateStudentRequest request);
    Task<GetStudentsResponse> Get(GetStudentsQuery request);
    Task<GetStudentsResponse> GetById(int id);
    Task<UpdateStudentResponse> Update(int id, UpdateStudentRequest request);
    Task<DeleteStudentResponse> Delete(int id);
}