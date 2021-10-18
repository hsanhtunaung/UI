using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace UI.Models
{
    public partial class ProgramTestContext : DbContext
    {
        public ProgramTestContext()
        {
        }

        public ProgramTestContext(DbContextOptions<ProgramTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblBuyType> TblBuyTypes { get; set; }
        public virtual DbSet<TblEVoucher> TblEVouchers { get; set; }
        public virtual DbSet<TblPaymentMethod> TblPaymentMethods { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
// To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=HSAN;Initial Catalog=ProgramTest;User Id=sa;Password=123456;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TblBuyType>(entity =>
            {
                entity.ToTable("Tbl_BuyType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblEVoucher>(entity =>
            {
                entity.ToTable("Tbl_eVoucher");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.BuyTypeId).HasColumnName("BuyTypeID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                //entity.HasOne(d => d.BuyType)
                //    .WithMany(p => p.TblEVouchers)
                //    .HasForeignKey(d => d.BuyTypeId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Tbl_eVoucher_Tbl_BuyType");

                //entity.HasOne(d => d.PaymentMethod)
                //    .WithMany(p => p.TblEVouchers)
                //    .HasForeignKey(d => d.PaymentMethodId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Tbl_eVoucher_Tbl_PaymentMethod");
            });

            modelBuilder.Entity<TblPaymentMethod>(entity =>
            {
                entity.ToTable("Tbl_PaymentMethod");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MethodName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserInfo__1788CC4C5377DA1C");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

             
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
