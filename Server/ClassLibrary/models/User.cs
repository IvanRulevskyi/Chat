namespace ClassLibrary.models;

public class User
{
    public int Id { get; set; }
    public string LoginUser { get; set; } = string.Empty;
    public string PasswordUser { get; set; } = string.Empty;
    public ICollection<UserContact>? UserContactsAll { get; set; } = new List<UserContact>();
}
