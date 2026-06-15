
using GymManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            // Table Name
            builder.ToTable("SubscriptionPlans");

            // Primary Key
            builder.HasKey(sp => sp.Id);

            // Properties
            builder.Property(sp => sp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sp => sp.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(sp => sp.DurationDays)
                .IsRequired();

            // Unique index on Name
            builder.HasIndex(sp => sp.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

        }
    }
}