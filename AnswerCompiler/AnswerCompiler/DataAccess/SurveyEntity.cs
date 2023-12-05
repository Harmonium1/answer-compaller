namespace AnswerCompiler.DataAccess;

public class SurveyEntity
{
    public Guid SurveyId { get; set; }
    public string AuthorId { get; set; } = null!;
    public int EnterNumber { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Closed { get; set; }
    public int VariantsAmount { get; set; }
    public List<string> AppliedUserIds { get; set; } = new();
    public List<SurveyAnswerEntity> Answers { get; set; } = new();

    public static int RandomNumber() => new Random().Next(10000);
}