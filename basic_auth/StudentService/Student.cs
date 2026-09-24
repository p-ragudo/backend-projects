using System.ComponentModel.DataAnnotations;

namespace basic_auth.StudentService;

public class Student
{
    [Required]
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsEnrolled { get; set; } = false;
}