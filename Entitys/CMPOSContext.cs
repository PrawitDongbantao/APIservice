using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using APIservice.Models;

#nullable disable

namespace APIservice.Entity
{
    public partial class CMPOSContext : DbContext
    {
        public CMPOSContext()
        {
        }

        public CMPOSContext(DbContextOptions<CMPOSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.BuyerId).HasColumnName("buyer_id");

                entity.Property(e => e.EmployeeId)
                    .IsRequired()
                    .HasColumnName("employee_id");

                entity.Property(e => e.OrderList)
                    .IsRequired()
                    .HasColumnName("order_list");

                entity.Property(e => e.PaymentDetail).HasColumnName("payment_detail");

                entity.Property(e => e.PaymentType).HasColumnName("payment_type");

                entity.Property(e => e.SellerId).HasColumnName("seller_id");

                entity.Property(e => e.ShippingCost).HasColumnName("shipping_cost");

                entity.Property(e => e.TaxPercent).HasColumnName("tax_percent");

                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "AK_Users_Username")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Position).HasDefaultValueSql("(N'Cashier')");

                entity.Property(e => e.Username).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
