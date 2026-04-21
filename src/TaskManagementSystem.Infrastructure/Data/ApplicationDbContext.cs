using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureIdentityTableNames(builder);
        ConfigureTaskItem(builder);
    }

    private static void ConfigureIdentityTableNames(ModelBuilder builder)
    {
        builder.Entity<IdentityUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
    }

    private static void ConfigureTaskItem(ModelBuilder builder)
    {
        builder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Description)
                .HasMaxLength(4000);

            entity.Property(t => t.Priority)
                .IsRequired();

            entity.Property(t => t.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasIndex(t => t.UserId);

            entity.Ignore(t => t.IsOverdue);
        });
    }
}
