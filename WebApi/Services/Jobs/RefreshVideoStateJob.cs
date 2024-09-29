using Microsoft.EntityFrameworkCore;
using Quartz;
using WebApi.Models;

namespace WebApi.Services.Jobs;


public class RefreshVideoStateJob(ApplicationDbContext db, IMlService mlService): IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var videosToCheckForDownloading = await db.Videos
            .Where(v => v.State == BasicVideo.ProcessingState.ProcessingInMl)
            .ToArrayAsync();
        foreach (var video in videosToCheckForDownloading)
        {
            try
            {
                
                var (state, highlights) = await mlService.GetHighlightsAsync(video.MlId!);
                if (state)
                {
                    video.State = BasicVideo.ProcessingState.Ready;
                    video.Highlights = highlights!.ToList();
                }
            }
            catch (Exception e){}
        }
        await db.SaveChangesAsync();
    }
}