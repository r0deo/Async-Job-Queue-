using System.Collections.Concurrent;
using AsyncJobQueue;

var builder = WebApplication.CreateBuilder(args);

ConcurrentDictionary<string, Job> jobStatuses = new ConcurrentDictionary<string, Job>();
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

class JobWorker : BackgroundService
{
    private readonly ConcurrentDictionary<string, Job> _jobStatuses;

    public JobWorker(ConcurrentDictionary<string, Job> jobStatuses)
    {
        _jobStatuses = jobStatuses;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        foreach (var entry in _jobStatuses)
        {
            if (entry.Value.status == "Queued")
            {
                var jobId = entry.Key;
                _jobStatuses[jobId] = new Job(jobId, "Processing");
                _ = CompleteJobAsync(jobId);
            }
        }
        await Task.Delay(500, stoppingToken); 
    }
}

private async Task CompleteJobAsync(string jobId)
{
    await Task.Delay(TimeSpan.FromSeconds(40));
    _jobStatuses[jobId] = new Job(jobId, "Completed");
}

}

