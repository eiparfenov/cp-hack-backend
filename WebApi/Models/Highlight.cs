using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Highlight
{
    [JsonPropertyName("start")] public int Start { get; set; }

    [JsonPropertyName("end")] public int End { get; set; }

    [JsonPropertyName("virality")] public float Virality { get; set; }

    [JsonPropertyName("file")] public string File { get; set; } = default!;
    [JsonPropertyName("transcription")] public List<Transcription> Transcriptions { get; set; } = default!;
}

public class Transcription 
{
    [JsonPropertyName("start")] public float Start { get; set; }
    [JsonPropertyName("end")] public float End { get; set; }
    [JsonPropertyName("text")] public string Text { get; set; } = default!;
}