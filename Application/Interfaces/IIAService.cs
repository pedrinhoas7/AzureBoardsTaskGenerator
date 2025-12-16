public interface IIAService
{
    Task<List<GeneratedTask>> GenerateTasksAsync(string cardTitle, string description);
}
