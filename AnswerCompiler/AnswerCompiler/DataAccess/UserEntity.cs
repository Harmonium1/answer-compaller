namespace AnswerCompiler.DataAccess;

public class UserEntity
{
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
    
    public DateTimeOffset Created { get; set; }

    private UserEntity() { }

    public UserEntity(string userId)
    {
        UserId = userId;
        Status = UserStatus.ProfileCreate;
        Role = UserRole.Student;
        Created = DateTimeOffset.Now;
    }
    
    public bool IsProfileFilled => !string.IsNullOrEmpty(Name);
}