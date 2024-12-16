using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ClassLibrary.Configurations;
using ClassLibrary.models;
namespace ClassLibrary.data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {

    }

    public DbSet<User> Logins => Set<User>();
    public DbSet<UserContact> UserContacts => Set<UserContact>();
    public DbSet<Message> Messages => Set<Message>();
    public static ILoggerFactory? MyloggerFactory = LoggerFactory.Create(configure =>
    {
        configure.AddConsole();
    });

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
        string? connStr = config.GetConnectionString("SQLServerConnections");
        optionsBuilder.UseSqlServer(connStr);
        optionsBuilder.UseLoggerFactory(MyloggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserContactConfiguration());
    }


}
