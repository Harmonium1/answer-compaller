using System.Text.Json.Serialization;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace AnswerCompiler.LineApi.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextMessage), typeDiscriminator: "text")]
public class Message
{
    protected string Type { get; } = null!;
}

public class TextMessage : Message
{
    public TextMessage(string text)
    {
        Text = text;
    }
    public string Text { get; set;}
    public string? QuoteToken { get; set;}
}
