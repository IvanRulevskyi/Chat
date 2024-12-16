using System.ComponentModel;
using ClassLibrary.models;
using Microsoft.EntityFrameworkCore;
namespace ClassLibrary.data;
public class Services
{
    private readonly ApplicationContext _context;
    public Services(ApplicationContext context)
    {
        _context = context;
    }
    public async Task AddUserAsync(string loginUser, string passwordUser)
    {
        if (await _context.Logins.AnyAsync(u =>u.LoginUser == loginUser) == false)
        {
            User login = new()
            {
                LoginUser = loginUser,
                PasswordUser = passwordUser
            };

            await _context.AddAsync(login);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddMessageAsync(string text, int idUser, int idContact, string userName, string contactName)
    {
        Message message = new()
        {
            DateTime = DateTime.Now,
            Text = text,
            SenderId = idUser,
            RecipientId = idContact,
            SenderName = userName,
            RecipientName = contactName
        };
        await _context.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ValidatePasswordAsync(string loginUser, string passwordUser)
    {
        return await _context.Logins
        .AnyAsync(t => t.LoginUser == loginUser && t.PasswordUser == passwordUser);
    }

    public async Task AddContactAsync(int userId, int contactId)
    {
        UserContact userContact = new()
        {
            UserId = userId,
            ContactId = contactId,
        };
        UserContact reverseContact = new()
        {
            UserId = contactId,
            ContactId = userId,
        };
        await _context.UserContacts.AddRangeAsync(userContact, reverseContact);
        await _context.SaveChangesAsync();
    }



}
