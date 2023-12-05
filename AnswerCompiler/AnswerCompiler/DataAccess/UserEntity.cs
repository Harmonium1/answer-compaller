namespace AnswerCompiler.DataAccess;

public class UserEntity
{
    public string LineUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
}