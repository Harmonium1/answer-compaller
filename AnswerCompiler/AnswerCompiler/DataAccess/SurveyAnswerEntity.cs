namespace AnswerCompiler.DataAccess;

public class SurveyAnswerEntity
{
    public Guid AnswerId { get; set; }
    public int QuestionId { get; set; }
    public string AuthorId { get; set; } = null!;
    public string Value { get; set; } = null!;
}