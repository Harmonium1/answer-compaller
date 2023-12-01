namespace AnswerCompiler.DataAccess;

public class Survey
{
    public Guid SurveyId { get; set; }
    public string AuthorId { get; set; } = null!;
    public int EnterNumber { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Closed { get; set; }
    public int VariantsAmount { get; set; }

    public static int RandomNumber() => new Random().Next(10000);
}