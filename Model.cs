namespace AsyncJobQueue;
 
public record Job(string Id,string status);

public record JobMetadata(string ReportType);