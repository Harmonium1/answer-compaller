﻿// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AnswerCompiler.LineApi.Models;

public class RequestBody
{
    public string Destination { get; set; } = null!;
    public IReadOnlyCollection<BaseEvent>? Events { get; set; }
}

