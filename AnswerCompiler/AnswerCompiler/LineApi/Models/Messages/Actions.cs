using System.Text.Json.Serialization;

namespace AnswerCompiler.LineApi.Models.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PostbackAction), typeDiscriminator: "postback")]
public class MessageAction
{
    protected string Type { get; set; } = null!;
}

public class PostbackAction : MessageAction
{
    public string? Label { get; set; }
    public string Data { get; set; } = null!;
    public string? DisplayText { get; set; }
    public string? InputOption { get; set; }
    public string? FillInText { get; set; }
}