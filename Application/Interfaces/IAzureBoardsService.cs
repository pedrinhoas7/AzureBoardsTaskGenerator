public interface IAzureBoardsService
{
    Task<CardInfo> GetCardInfoAsync(int cardId);
    Task<int> CreateTaskAsync(int parentId, string title, string description);
}
