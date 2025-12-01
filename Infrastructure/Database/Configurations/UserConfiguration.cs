using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("UserId")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .HasColumnName("Email")            
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        
        builder.Property(x => x.DisplayName)
            .HasColumnName("DisplayName")
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        
        builder.Property(x => x.AuthProvider)
            .HasColumnName("AuthProvider")
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        
        builder.Property(x => x.ProviderSubject)
            .HasColumnName("ProviderSubject")
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        
        builder.HasIndex(x => new { x.AuthProvider, x.ProviderSubject }).IsUnique();
        builder.HasIndex(x => x.Email);
        
        

    }
}