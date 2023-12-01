using System.Text.Json.Serialization;

namespace AnswerCompiler.LineApi.Models.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ButtonsTemplate), typeDiscriminator: "buttons")]
public abstract class Template
{
    public string Type { get; set; } = null!;
}
public class ButtonsTemplate : Template
{
    public string? ThumbnailImageUrl { get; set; }
    public string? ImageAspectRatio { get; set; }
    public string? ImageSize { get; set; }
    public string? ImageBackgroundColor { get; set; }
    public string? Title { get; set; }
    public string Text { get; set; } = null!;
    public MessageAction? DefaultAction { get; set; }
    public List<MessageAction> Actions { get; set; } = new();
}