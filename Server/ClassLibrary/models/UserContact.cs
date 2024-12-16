namespace ClassLibrary.models;

public class UserContact
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? MyUser { get; set; }

    public int ContactId { get; set; }
    public User? ContactUser { get; set; }
}
