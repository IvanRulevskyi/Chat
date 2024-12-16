using ClassLibrary.data;
using ClassLibrary.models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<Services>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.EnsureCreated();
}

// Add profile
app.MapPost("/usercreation", async (User user, ApplicationContext context, Services services) =>
{

    if (user == null || string.IsNullOrEmpty(user.LoginUser) || string.IsNullOrEmpty(user.PasswordUser))
    {
        return Results.BadRequest("Invalid input");
    }
    else
    {
        bool userExists = await context.Logins
        .AnyAsync(t => t.LoginUser == user.LoginUser);
        if (userExists)
        {
            return Results.Ok("A user with this name already exists!");
        }
        await services.AddUserAsync(user.LoginUser, user.PasswordUser);
        return Results.Ok($"User created: {user.LoginUser}");
    }
});

// valid profile
app.MapPost("/user", async (User user, Services services) =>
{    
    if (user == null || string.IsNullOrEmpty(user.LoginUser) || string.IsNullOrEmpty(user.PasswordUser))
    {
        return Results.BadRequest("Invalid input");
    }
    bool isValidUser = await services.ValidatePasswordAsync(user.LoginUser, user.PasswordUser);
    if (isValidUser)
    {
        return Results.Ok(true);
    }
    else
    {
        return Results.Ok(false);
    }
});

// Search User For Add in contact
app.MapGet("/searchuser", async (ApplicationContext context) =>
{
    var search = await context.Logins.Select(u => new{Id = u.Id, LoginUser = u.LoginUser}).ToListAsync();
    return Results.Ok(search);
});





// Search Contact
app.MapPost("/searchcontact", async (SearchUser searchUser, ApplicationContext context, Services services) =>
{
    if (searchUser == null)
    {
        return Results.BadRequest("Invalid input");
    }
    var userId = await context.Logins
        .Where(uId => uId.LoginUser == searchUser.LoginUser)
        .Select(u => u.Id)
        .FirstOrDefaultAsync();

    var contacts = await context.UserContacts
        .Include(uc => uc.ContactUser)
        .Where(uc => uc.UserId == userId)
        .Select(uc => new
        {
            uc.ContactId,
            ContactName = uc.ContactUser.LoginUser,
        })
        .ToListAsync();

    return Results.Ok(contacts);
});

// Add contact
app.MapPost("/addcontact", async (UserContact userContact,ApplicationContext context, Services services) =>
{
    if (userContact == null)
    {
        return Results.BadRequest("Invalid input");
    }
    bool contactExists = await context.UserContacts
    .AnyAsync(c => 
        (c.UserId == userContact.UserId && c.ContactId == userContact.ContactId) || 
        (c.UserId == userContact.ContactId && c.ContactId == userContact.UserId));

    if (contactExists)
    {
        return Results.Ok("Contact already added or exists in reverse.");
    }
    await services.AddContactAsync(userContact.UserId, userContact.ContactId);
    return Results.Ok("Contact added");
});


app.MapGet("/searchmessages/{nameSendMessage}/{userName}", async (string nameSendMessage, string userName, ApplicationContext context) =>
{
    int idUser = await context.Logins
        .Where(u => u.LoginUser == userName)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();

    int idContact = await context.Logins
        .Where(u => u.LoginUser == nameSendMessage)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();
    var messages = await context.Messages
        .Where(m => (m.SenderId == idUser && m.RecipientId == idContact) ||
                    (m.SenderId == idContact && m.RecipientId == idUser))
        .Select(m => new
        {
            m.Id,
            m.DateTime,
            m.Text,
            SenderName = (m.SenderId == idUser) ? userName : nameSendMessage,
            RecipientName = (m.RecipientId == idUser) ? userName : nameSendMessage
        })
        .OrderBy(m => m.DateTime)
        .ToListAsync();
    return Results.Ok(messages);
});

app.MapDelete("/deletecontact/{contactName}/{userName}", async (string contactName, string userName, ApplicationContext context) =>
{

    int idUser = await context.Logins
        .Where(u => u.LoginUser == userName)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();

    int idContact = await context.Logins
        .Where(u => u.LoginUser == contactName)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();

    
    var contact = await context.UserContacts.FirstOrDefaultAsync(uc => uc.UserId == idUser && uc.ContactId == idContact);
    if (contact == null)
    {
        return Results.NotFound("Contact not found");
    }

    context.UserContacts.Remove(contact);
    await context.SaveChangesAsync();
    return Results.Ok("Contact deleted successfully");
});

app.MapPost("/addmessage", async (AddMessage addMessage, ApplicationContext context, Services services) =>
{
    if (addMessage == null)
    {
        return Results.NotFound("Contact not found");
    }

    int idUser = await context.Logins
        .Where(u => u.LoginUser == addMessage.userName)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();

    int idContact = await context.Logins
        .Where(u => u.LoginUser == addMessage.contactName)
        .Select(u=> u.Id)
        .FirstOrDefaultAsync();
    await services.AddMessageAsync(addMessage.textMessage, idUser, idContact,addMessage.userName, addMessage.contactName);
    return Results.Ok("Message Send");
});

await app.RunAsync();
public record SearchUser(string LoginUser);
public record DeleteContactUser(int ContactId);
public record AddMessage(string userName, string contactName, string textMessage);
// cd /Users/ivanrulevskiy/Desktop/WL/Network\ programming/Chat/Server/ChatServer
// dotnet run --launch-profile "https"