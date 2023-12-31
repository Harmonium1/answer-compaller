﻿// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using AnswerCompiler.LineApi.Models.Events;

namespace AnswerCompiler.LineApi.Models;

public class WebhookBody
{
    public string Destination { get; set; } = null!;
    public IReadOnlyCollection<BaseEvent>? Events { get; set; }
}


