using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using WebApi;
using WebApi.Models;
using WebApi.Options;
using WebApi.Services;
using WebApi.Services.Initialize;
using WebApi.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.Configure<S3Options>(builder.Configuration.GetSection(nameof(S3Options)));
builder.Services.Configure<UrlGenerationOptions>(builder.Configuration.GetSection(nameof(UrlGenerationOptions)));

builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb"));
    o.UseSnakeCaseNamingConvention(); 
});
builder.Services.AddSingleton(s =>
{
    var opts = s.GetRequiredService<IOptions<S3Options>>().Value;
    return new AmazonS3Client(opts.AccessKeyId, opts.SecretAccessKey, new AmazonS3Config()
    {
        ServiceURL = opts.ServiceUrl,
        ForcePathStyle = true
    });
});
builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<RefreshVideoStateJob>(opts => opts
        .WithIdentity(nameof(RefreshVideoStateJob) + "-trigger")
        .StartNow()
        .WithCronSchedule("0/5 * * * * ?"));
});
builder.Services.AddQuartzHostedService();
builder.Services.AddCors();

builder.Services.AddScoped<RefreshVideoStateJob>();
builder.Services.AddScoped<IVideoCreationService, VideoCreationService>();
builder.Services.AddHttpClient<IMlService, MlService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MlService")!);
});
builder.Services.AddHostedService<CreateBucketService>();
builder.Services.AddHostedService<MigrateDb<ApplicationDbContext>>();

var app = builder.Build();

app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/api/videoId", () => new {VideoId = Guid.NewGuid().ToString()});
app.MapPost("/api/postVideo", async (
    [FromForm]string title, 
    [FromForm]string id, 
    [FromForm]int clipsCount,  
    [FromForm]IFormFile video,
    [FromServices] IVideoCreationService videoCreationService) =>
{
    var videoId = Guid.Parse(id);
    await videoCreationService.CreateVideoAsync(videoId, title, video, clipsCount);
}).DisableAntiforgery();
app.MapGet("/api/listVideos", async ([FromServices] ApplicationDbContext db) =>
{
    var videos = await db.Videos
        .Select(v => v.Id)
        .ToListAsync();
    return new { Videos = videos };
});
app.MapGet("/api/getVideo", async ([FromQuery] string videoId, [FromServices] ApplicationDbContext db, [FromServices] IOptions<UrlGenerationOptions> urlOptions) =>
{
    if(!Guid.TryParse(videoId, out Guid id)) return Results.BadRequest("videoId is not guid");
    var video = await db.Videos.AsNoTracking().SingleOrDefaultAsync(v => v.Id == id);
    if (video == null) return Results.NotFound();
    return Results.Json(new
    {
        Status = video.State, 
        Highlights = video.Highlights,
        Link = urlOptions.Value.LinkToVideoTemplate.Replace("{videoId}", video.Id.ToString()),
        Title = video.Title, 
    });
});
app.Run();