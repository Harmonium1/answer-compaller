namespace AnswerCompiler.DataAccess;

public class SurveyEntity
{
    public Guid SurveyId { get; set; }
    public string AuthorId { get; set; } = null!;
    public int EnterNumber { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Closed { get; set; }
    public int VariantsAmount { get; set; }
    public List<string> AppliedUserIds { get; set; } = new();
    public List<SurveyAnswerEntity> Answers { get; set; } = new();
    
    private const int DefaultQuestionAmount = 4;
    private SurveyEntity() { }

    public SurveyEntity(string authorId, int? variantsAmount)
    {
        SurveyId = Guid.NewGuid();
        AuthorId = authorId;
        EnterNumber = GenerateSurveyNumber();
        Created = DateTimeOffset.Now;
        VariantsAmount = variantsAmount ?? DefaultQuestionAmount;
    }
    
    private static int GenerateSurveyNumber() => new Random().Next(10000);
    public int LastQuestionId => Answers.Count != 0 ? Answers.Select(a => a.QuestionId).Max() : 0;
}