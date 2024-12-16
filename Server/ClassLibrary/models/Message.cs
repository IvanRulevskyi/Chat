namespace ClassLibrary.models;

public class Message
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Text { get; set; } = "";

    public int SenderId { get; set; }
    public int RecipientId { get; set; }

    public string SenderName { get; set; } = "";
    public string RecipientName { get; set; } = "";
}
