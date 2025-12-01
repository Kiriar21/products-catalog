using Domain;
using Domain.ProductRecords;
using Domain.ProductSpecificationRecords;
using Domain.TaxonomyRecords;
using Domain.VersionProductRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "dbo");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property<DateTimeOffset?>("CreatedAt")
            .HasColumnName("CreatedAt")
            .HasColumnType("datetimeoffset(0)");

        builder.Property<string?>("CreatedBy")
            .HasColumnName("CreatedBy")
            .HasMaxLength(255);

        builder.Property<DateTimeOffset?>("ModifiedAt")
            .HasColumnName("ModifiedAt")
            .HasColumnType("datetimeoffset(0)");
        
        builder.Property<string?>("ModifiedBy")
            .HasColumnName("ModifiedBy")
            .HasMaxLength(255);

        builder.OwnsOne(p => p.Type, typ =>
            typ.Property(x => x.Name).HasMaxLength(64).IsUnicode(false).IsRequired());

        builder.OwnsOne(p => p.LifeCycleStatus, status =>
            status.Property(x => x.Name).HasMaxLength(64).IsUnicode(false).IsRequired());

        builder.OwnsMany<VersionProduct>(p => p.Versions, version =>
        {
            version.ToTable("ProductVersions", "dbo");
            version.WithOwner().HasForeignKey("ProductId");
            
            version.HasKey("Id");
            
            version.Property<Guid>("Id")
                .HasColumnType("uniqueidentifier")
                .ValueGeneratedNever();

            version.Property(x => x.IsObsolete).IsRequired();
            
            version.OwnsOne(v => v.Name, name =>
                name.Property(p => p.Name).HasMaxLength(128).IsRequired());

            version.OwnsOne(v => v.VersionNumber, num =>
                num.Property(p => p.Number).HasColumnType("int").IsRequired());


            version.Property<DateTimeOffset?>("CreatedAt")
                .HasColumnName("CreatedAt")
                .HasColumnType("datetimeoffset(0)");

            version.Property<string?>("CreatedBy")
                .HasColumnName("CreatedBy")
                .HasMaxLength(255);

            version.Property<DateTimeOffset?>("ModifiedAt")
                .HasColumnName("ModifiedAt")
                .HasColumnType("datetimeoffset(0)");

            version.Property<string?>("ModifiedBy")
                .HasColumnName("ModifiedBy")
                .HasMaxLength(255);

            version.OwnsOne(x => x.Spec, spec =>
                {
                    spec.ToTable("ProductSpecifications", "dbo"); 
                    spec.WithOwner().HasForeignKey("VersionId");
            
                    spec.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .ValueGeneratedNever()
                        .IsRequired();
                    
                    spec.HasKey("Id");

                    spec.OwnsOne(s => s.Price, p =>
                        p.Property(x => x.Value).HasColumnType("decimal(18,2)").IsRequired());

                    spec.OwnsOne(s => s.Amount, a =>
                        a.Property(x => x.Value).HasColumnType("decimal(18,3)").IsRequired());
                    
                    spec.OwnsOne(s => s.Taxonomy, tax =>
                    {
                        tax.OwnsOne(t => t.ProductCategory, pc =>
                            pc.Property(x => x.Name).HasMaxLength(64).IsUnicode(false).IsRequired());
                        tax.OwnsOne(t => t.GenerationRecord, gr =>
                            gr.Property(x => x.Name).HasMaxLength(32).IsUnicode(false).IsRequired());
                        tax.OwnsOne(t => t.Kind, k =>
                            k.Property(x => x.Name).HasMaxLength(32).IsUnicode(false).IsRequired());
                        tax.WithOwner();
                    });

                    spec.OwnsMany<Resource>("_resources", res =>
                    {
                        res.ToTable("ProductSpecificationResources", "dbo");
                        res.WithOwner().HasForeignKey("SpecId");
                        res.HasKey("SpecId", "Key");
                        res.Property<string>("Key").HasMaxLength(64).IsUnicode(false).IsRequired();
                        res.Property<string>("Value").HasMaxLength(2048).IsUnicode().IsRequired();
                    });
                    

                });
        });
        
        builder.Navigation(p => p.Versions).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Product.Versions))!
            .SetField("_versions");
        
    }
}