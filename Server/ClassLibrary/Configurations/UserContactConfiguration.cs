using ClassLibrary.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassLibrary.Configurations
{
    public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.HasOne(uc => uc.MyUser) 
                   .WithMany(u => u.UserContactsAll) 
                   .HasForeignKey(uc => uc.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uc => uc.ContactUser) 
                   .WithMany() 
                   .HasForeignKey(uc => uc.ContactId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
