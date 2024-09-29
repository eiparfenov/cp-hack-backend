using System.Text.Json.Serialization;
using WebApi.Models;

namespace WebApi.Services;

public interface IMlService
{
    Task<string> SendVideo(string videoUrl, string title, int clipsCount);
    Task<(bool isSuccess, IReadOnlyCollection<Highlight>? highlights)> GetHighlightsAsync(string videoId);
}

public class MlService(HttpClient client) : IMlService
{
    public async Task<string> SendVideo(string videoUrl, string title, int clipsCount)
    {
        var response = await client.PostAsJsonAsync("/set_video_download", new MlSendVideoRequest()
        {
            DownloadUrl = videoUrl,
            Filename = title,
            ClipsCount = clipsCount
        });
        response.EnsureSuccessStatusCode();
        
        var mlResponse = await response.Content.ReadFromJsonAsync<MlSendVideoResponse>();
        return mlResponse!.TaskId;
    }

    public async Task<(bool isSuccess, IReadOnlyCollection<Highlight>? highlights)> GetHighlightsAsync(string videoId)
    {
        var response = await client.PostAsJsonAsync($"/task_status", new MlGetTaskRequest()
        {
            TaskId = videoId
        });
        var mlResponse = await response.Content.ReadFromJsonAsync<MlGetTaskResponse>();
        return (mlResponse!.Highlights != null, mlResponse.Highlights);
    }
    
    private class MlGetTaskRequest
    {
        [JsonPropertyName("task_id")] public string TaskId { get; set; } = default!;
    }

    private class MlGetTaskResponse
    {
        [JsonPropertyName("highlights")] public IReadOnlyCollection<Highlight>? Highlights { get; set; }
    }

    private class MlSendVideoRequest
    {
        [JsonPropertyName("download_url")] public required string DownloadUrl { get; set; }
        
        [JsonPropertyName("filename")] public required string Filename { get; set; }
        [JsonPropertyName("clip_count")] public int ClipsCount { get; set; }
    }

    private class MlSendVideoResponse
    {
        [JsonPropertyName("task_id")] public required string TaskId { get; set; }
    }
}