namespace AnswerCompiler.DataAccess;

public class SurveyAnswerEntity
{
    public Guid AnswerId { get; set; }
    public int QuestionId { get; set; }
    public string AuthorId { get; set; } = null!;
    public string Value { get; set; } = null!;
    
    private SurveyAnswerEntity() { }

    public SurveyAnswerEntity(UserEntity author, int questionId, string value)
    {
        AnswerId = Guid.NewGuid();
        AuthorId = author.UserId;
        QuestionId = questionId;
        Value = value;
    }
}