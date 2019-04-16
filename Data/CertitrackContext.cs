using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Certitrack
{
    public partial class CertitrackContext : DbContext
    {
        public CertitrackContext()
        {
        }

        public CertitrackContext(DbContextOptions<CertitrackContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Certificate> Certificate { get; set; }
        public virtual DbSet<CertificateLink> CertificateLink { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffLink> StaffLink { get; set; }
        public virtual DbSet<StaffType> StaffType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Certitrack;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("certificate");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CertificateNo)
                    .HasColumnName("certificate_no")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.DateIssued)
                    .HasColumnName("DATE_issued")
                    .HasColumnType("date")
                    .HasDefaultValueSql("(CONVERT([date],getdate()))");

                entity.Property(e => e.DateRedeemed)
                    .HasColumnName("DATE_redeemed")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<CertificateLink>(entity =>
            {
                entity.HasKey(e => e.CertificateId);

                entity.ToTable("certificate_link");

                entity.Property(e => e.CertificateId)
                    .HasColumnName("certificate_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.Certificate)
                    .WithOne(p => p.CertificateLink)
                    .HasForeignKey<CertificateLink>(d => d.CertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_certificate_cLink");

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.CertificateLink)
                    .HasForeignKey(d => d.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_channel_cLink");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CertificateLink)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_customer_cLink");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.CertificateLink)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("FK_promotion_cLink");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.CertificateLink)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_staff_cLink");
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.ToTable("channel");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Channel1)
                    .IsRequired()
                    .HasColumnName("channel")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_customer_order");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_item");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CertificateId).HasColumnName("certificate_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.HasOne(d => d.Certificate)
                    .WithMany(p => p.OrderItem)
                    .HasForeignKey(d => d.CertificateId)
                    .HasConstraintName("FK_certificate_oItem");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItem)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_oItem");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Discount).HasColumnName("discount");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staff");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StaffLink>(entity =>
            {
                entity.HasKey(e => e.StaffId);

                entity.ToTable("staff_link");

                entity.Property(e => e.StaffId)
                    .HasColumnName("staff_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.StaffTypeId).HasColumnName("staff_type_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.StaffLink)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_sLink");

                entity.HasOne(d => d.Staff)
                    .WithOne(p => p.StaffLink)
                    .HasForeignKey<StaffLink>(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_staff_sLink");

                entity.HasOne(d => d.StaffType)
                    .WithMany(p => p.StaffLink)
                    .HasForeignKey(d => d.StaffTypeId)
                    .HasConstraintName("FK_staff_type_sLink");
            });

            modelBuilder.Entity<StaffType>(entity =>
            {
                entity.ToTable("staff_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });
        }
    }
}
