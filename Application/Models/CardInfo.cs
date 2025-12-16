public class CardInfo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public CardInfo(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }
}
