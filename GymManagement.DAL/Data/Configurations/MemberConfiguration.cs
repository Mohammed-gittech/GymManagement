
using GymManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            // Table Name 
            builder.ToTable("Members");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Property
            builder.Property(m => m.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Email)
                .HasMaxLength(100);

            // index
            builder.HasIndex(m => m.Phone)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(m => m.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}