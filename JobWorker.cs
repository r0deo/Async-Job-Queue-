using System.Collections.Concurrent;

namespace AsyncJobQueue;

public class JobWorker : BackgroundService
{
    private readonly ConcurrentDictionary<string, Job> _jobStatuses;

    public  JobWorker(ConcurrentDictionary<string, Job> jobStatuses)
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
