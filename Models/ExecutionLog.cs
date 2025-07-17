namespace WorkflowEngine.Models;

public class ExecutionLog
{
    public string ActionId { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
