

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Dictionary<string, Job> jobStatuses = new Dictionary<string, Job>();

app.MapPost("/jobs", (JobMetadata jobMetadata) =>
{
    var jobId = Guid.NewGuid().ToString();
    jobStatuses.Add(jobId, new(jobId, "Queued"));

    return Results.Ok(new { jobId, status = "Queued" });
});

app.MapGet("/jobs/{jobId}", (string jobid) =>
{

})

app.Run();


record Job(string Id,string status);

record JobMetadata(string ReportType);