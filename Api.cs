using AsyncJobQueue;
using System.Collections.Concurrent;

ConcurrentDictionary<string, Job> jobStatuses = new ConcurrentDictionary<string, Job>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(jobStatuses);
builder.Services.AddHostedService<JobWorker>();

var app = builder.Build();

app.MapPost("/jobs", (JobMetadata jobMetadata) =>
{
    var jobId = Guid.NewGuid().ToString();

     jobStatuses.TryAdd(jobId, new Job(jobId, "Queued"));
    
    return Results.Ok(new { jobId, status = "Queued" });
});

app.MapGet("/jobs/{jobId}", (string jobId) =>
{
    if (!jobStatuses.TryGetValue(jobId, out var job))
    {
        return Results.NotFound(new { jobId, status = "Not Found" });
    }
    return Results.Ok(job);
});

app.Run();



