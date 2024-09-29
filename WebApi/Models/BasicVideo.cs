using System.Text.Json.Serialization;

namespace WebApi.Models;

public class BasicVideo
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? MlId { get; set; }
    public ProcessingState State { get; set; }
    public List<Highlight> Highlights { get; set; } = [];
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProcessingState
    {
        DownloadingToBackend,
        ProcessingInMl,
        Ready
    }
}