public class TaskManager
{
    private readonly IAzureBoardsService _azureBoardsService;
    private readonly IIAService _iaService;

    public TaskManager(IAzureBoardsService azureBoardsService, IIAService iaService)
    {
        _azureBoardsService = azureBoardsService;
        _iaService = iaService;
    }

    public async Task GenerateTasksForCardAsync(int cardId)
    {
        var cardInfo = await _azureBoardsService.GetCardInfoAsync(cardId);

        var generatedTasks = await _iaService.GenerateTasksAsync(cardInfo.Title, cardInfo.Description);

        var createdTasks = new List<(int Id, string Title)>();

        foreach (var task in generatedTasks)
        {
            var taskId = await _azureBoardsService.CreateTaskAsync(cardId, task.Title, task.Description);
            createdTasks.Add((taskId, task.Title));
        }

        foreach (var task in createdTasks)
        {
            Console.WriteLine($"Task created: [{task.Id}] {task.Title}");
        }
    }
}
