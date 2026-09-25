using Microsoft.EntityFrameworkCore;

namespace basic_auth.StudentService;

public class StudentService : IStudentService
{
    private readonly IDb _db;

    public StudentService(IDb db)
    {
        _db = db;    
    }

    public async Task<CreateStudentResponse> Create(CreateStudentRequest request)
    {
        var newStudent = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsEnrolled = false
        };

        await _db.Students.AddAsync(newStudent);
        await _db.SaveChangesAsync();

        return new CreateStudentResponse(
            newStudent.Id,
            newStudent.FirstName,
            newStudent.LastName,
            newStudent.IsEnrolled
        );
    }

    public async Task<GetStudentsResponse> Get(GetStudentsQuery request)
    {
        GetStudentsResponse response;
        // Get All
        if (
            request.Ids is { Count: <= 0} &&
            request.FirstNames is { Count: <= 0} &&
            request.LastNames is { Count: <= 0} &&
            request.IsEnrolled == null)
        {
            var allStudents = await _db.Students.ToListAsync();
            response = new GetStudentsResponse(allStudents);
            
            return response;       
        }


        // Get with query
        var query = _db.Students
            .AsNoTracking()
            .AsQueryable();
        
        if (request.Ids is { Count: > 0})
        {
            query = query.Where(q => request.Ids.Contains(q.Id));   
        }

        if (request.FirstNames is { Count: > 0})
        {
            var lowerFirstNames = request.FirstNames
                .Select(fn => fn.ToLower())
                .ToList();
            
            query = query.Where(q => lowerFirstNames.Contains(q.FirstName.ToLower()));
        }

        if (request.LastNames is { Count: > 0})
        {
            var lowerLastNames = request.LastNames
                .Select(ln => ln.ToLower())
                .ToList();

            query = query.Where(q => lowerLastNames.Contains(q.LastName.ToLower()));
        }

        if (request.IsEnrolled != null)
        {
            query = query.Where(q => q.IsEnrolled == request.IsEnrolled);
        }

        var filteredStudents = await query.ToListAsync();
        response = new GetStudentsResponse(filteredStudents);

        return response;
    }

    public async Task<GetStudentsResponse> GetById(int id)
    {
        GetStudentsResponse response = new([]);
        
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);

        if (student != null)
        {
            response.Students.Add(student);
        }

        return response;
    }
    
    public async Task<UpdateStudentResponse> Update(int id, UpdateStudentRequest request)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found");
        }

        student.FirstName = request.FirstName ?? student.FirstName;
        student.LastName = request.LastName ?? student.LastName;
        student.IsEnrolled = request.IsEnrolled ?? student.IsEnrolled;

        await _db.SaveChangesAsync();

        return new UpdateStudentResponse(
            student.Id,
            student.FirstName,
            student.LastName,
            student.IsEnrolled
        );
    }

    public async Task<DeleteStudentResponse> Delete(int id)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == id) 
            ?? throw new KeyNotFoundException($"Student with ID {id} not found.");

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        return new DeleteStudentResponse(
            student.Id,
            student.FirstName,
            student.LastName,
            student.IsEnrolled
        );
    }
}