using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using WebApi.Models;
using WebApi.Options;

namespace WebApi.Services;

public interface IVideoCreationService
{
    Task CreateVideoAsync(Guid videoId, string title, IFormFile file, int clipsCount);
}

public class VideoCreationService(
    ApplicationDbContext db,
    IMlService mlService,
    AmazonS3Client s3Client, 
    IOptions<S3Options> s3Options,
    IOptions<UrlGenerationOptions> urlOptions
) : IVideoCreationService
{
    public async Task CreateVideoAsync(Guid videoId, string title, IFormFile file, int clipsCount)
    {
        var video = new BasicVideo()
        {
            Id = videoId,
            Title = title,
            State = BasicVideo.ProcessingState.DownloadingToBackend
        };
        await db.Videos.AddAsync(video);
        await db.SaveChangesAsync();
       
        await using var fileStream = new MemoryStream();
        await file.CopyToAsync(fileStream);
        
        await s3Client.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = s3Options.Value.BucketName,
            Key = $"videos/{video.Id}.mp4",
            InputStream = fileStream,
            CannedACL = S3CannedACL.PublicRead
        });
        
        var urlToVideo = urlOptions.Value.LinkToVideoTemplate.Replace("{videoId}", video.Id.ToString());
        var mlId = await mlService.SendVideo(urlToVideo, title, clipsCount);
        
        video.MlId = mlId;
        video.State = BasicVideo.ProcessingState.ProcessingInMl;
        await db.SaveChangesAsync();
    }
}