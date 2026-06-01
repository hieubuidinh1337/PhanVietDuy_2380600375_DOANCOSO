using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Models.Domain;

namespace PhanVietDuy_2380600375.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiLog> ApiLogs { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContactMessage> ContactMessages { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductColor> ProductColors { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductSize> ProductSizes { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        // Bảng Wishlist mới — lưu sản phẩm yêu thích vào DB
        public virtual DbSet<WishlistItem> WishlistItems { get; set; }
        public virtual DbSet<VwOrderSummary> VwOrderSummaries { get; set; }
        public virtual DbSet<VwProductList> VwProductLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Critical for Identity

            modelBuilder.UseCollation("Vietnamese_CI_AS");

            modelBuilder.Entity<ApiLog>(entity =>
            {
                entity.HasIndex(e => e.CreatedAt, "IX_ApiLogs_CreatedAt").IsDescending();
                entity.HasIndex(e => e.Endpoint, "IX_ApiLogs_Endpoint");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Endpoint).HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.Method).HasMaxLength(10);
                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasIndex(e => e.SessionKey, "IX_CartItems_SessionKey");
                entity.HasIndex(e => e.UserId, "IX_CartItems_UserId");
                entity.Property(e => e.AddedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Quantity).HasDefaultValue(1);
                entity.Property(e => e.SelectedColor).HasMaxLength(100);
                entity.Property(e => e.SelectedSize).HasMaxLength(50);
                entity.Property(e => e.SessionKey).HasMaxLength(100);

                entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_CartItems_Products");

                entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CartItems_Users");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Slug, "IX_Categories_Slug");
                entity.HasIndex(e => e.Slug, "UQ_Categories_Slug").IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Label).HasMaxLength(150);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            });

            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(30);
                entity.Property(e => e.RepliedById).HasMaxLength(450);
                entity.Property(e => e.Subject).HasMaxLength(300);

                entity.HasOne(d => d.RepliedBy).WithMany(p => p.ContactMessages)
                    .HasForeignKey(d => d.RepliedById)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ContactMessages_Admin");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasIndex(e => e.Code, "UQ_Coupons_Code").IsUnique();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.DiscountType)
                    .HasMaxLength(20)
                    .HasDefaultValue("Percentage");
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<NewsletterSubscriber>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ_NewsletterSubscribers_Email").IsUnique();
                entity.Property(e => e.ConfirmToken).HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.SubscribedAt).HasDefaultValueSql("(sysutcdatetime())");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderedAt, "IX_Orders_OrderedAt").IsDescending();
                entity.HasIndex(e => e.Status, "IX_Orders_Status");
                entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

                entity.Property(e => e.CustomerEmail).HasMaxLength(256);
                entity.Property(e => e.CustomerName).HasMaxLength(200);
                entity.Property(e => e.CustomerPhone).HasMaxLength(30);
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Note).HasMaxLength(1000);
                
                // USER REQUIREMENT: Computed column with stored: true
                entity.Property(e => e.OrderCode)
                    .HasMaxLength(9)
                    .HasComputedColumnSql("N'VD-' + RIGHT(N'000000' + CAST([Id] AS NVARCHAR(6)), 6)", stored: true);
                
                entity.Property(e => e.OrderedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasDefaultValue("COD");
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(20)
                    .HasDefaultValue("Unpaid");
                entity.Property(e => e.ProcessedByAdminId).HasMaxLength(450);
                entity.Property(e => e.ShipAddress).HasMaxLength(500);
                entity.Property(e => e.ShipCity).HasMaxLength(100);
                entity.Property(e => e.ShipCountry)
                    .HasMaxLength(100)
                    .HasDefaultValue("Vietnam");
                entity.Property(e => e.ShipPostalCode).HasMaxLength(20);
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .HasDefaultValue("Pending");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ProcessedByAdmin).WithMany(p => p.OrderProcessedByAdmins)
                    .HasForeignKey(d => d.ProcessedByAdminId)
                    .HasConstraintName("FK_Orders_AdminUser");

                entity.HasOne(d => d.User).WithMany(p => p.OrderUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");
                entity.HasIndex(e => e.ProductId, "IX_OrderDetails_ProductId");

                entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                
                // USER REQUIREMENT: Computed column with stored: true
                entity.Property(e => e.LineTotal)
                    .HasComputedColumnSql("[UnitPrice] * [Quantity]", stored: true)
                    .HasColumnType("decimal(29, 2)");
                
                entity.Property(e => e.ProductName).HasMaxLength(200);
                entity.Property(e => e.ProductSku).HasMaxLength(100);
                entity.Property(e => e.Quantity).HasDefaultValue(1);
                entity.Property(e => e.SelectedColor).HasMaxLength(100);
                entity.Property(e => e.SelectedSize).HasMaxLength(50);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");
                entity.HasIndex(e => e.IsActive, "IX_Products_IsActive").HasFilter("([IsActive]=(1))");
                entity.HasIndex(e => e.IsFeatured, "IX_Products_IsFeatured").HasFilter("([IsFeatured]=(1))");
                entity.HasIndex(e => e.Price, "IX_Products_Price");
                entity.HasIndex(e => e.Slug, "IX_Products_Slug");
                entity.HasIndex(e => e.Slug, "UQ_Products_Slug").IsUnique();

                entity.Property(e => e.Badge).HasMaxLength(50);
                entity.Property(e => e.BadgeStyle)
                    .HasMaxLength(20)
                    .HasDefaultValue("default");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.CreatedByUserId).HasMaxLength(450);
                entity.Property(e => e.Dimensions).HasMaxLength(300);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Materials).HasMaxLength(1000);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ShippingInfo).HasMaxLength(1000);
                entity.Property(e => e.ShortDesc).HasMaxLength(500);
                entity.Property(e => e.Slug).HasMaxLength(200);
                entity.Property(e => e.ThumbnailPath).HasMaxLength(1000);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

                // USER REQUIREMENT: RowVersion / ConcurrencyCheck
                entity.Property(e => e.StockQuantity).IsConcurrencyToken();

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Categories");

                entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Products_CreatedBy");
            });

            modelBuilder.Entity<ProductColor>(entity =>
            {
                entity.Property(e => e.ColorName).HasMaxLength(100);
                entity.Property(e => e.HexCode)
                    .HasMaxLength(7)
                    .IsFixedLength();

                entity.HasOne(d => d.Product).WithMany(p => p.ProductColors)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductColors_Products");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductId");

                entity.Property(e => e.AltText).HasMaxLength(300);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.FilePath).HasMaxLength(1000);
                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductImages_Products");
            });

            modelBuilder.Entity<ProductSize>(entity =>
            {
                entity.Property(e => e.SizeName).HasMaxLength(50);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductSizes_Products");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.IsHomepage, "IX_Reviews_IsHomepage").HasFilter("([IsHomepage]=(1))");
                entity.HasIndex(e => new { e.ProductId, e.IsApproved }, "IX_Reviews_ProductApprv");
                entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId");

                entity.Property(e => e.AuthorName).HasMaxLength(150);
                entity.Property(e => e.AuthorTitle).HasMaxLength(200);
                entity.Property(e => e.Content).HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Reviews_Products");

                entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Reviews_Users");
            });

            modelBuilder.Entity<VwOrderSummary>(entity =>
            {
                entity.HasNoKey().ToView("vw_OrderSummary");
                entity.Property(e => e.CustomerEmail).HasMaxLength(256);
                entity.Property(e => e.CustomerName).HasMaxLength(200);
                entity.Property(e => e.CustomerPhone).HasMaxLength(30);
                entity.Property(e => e.OrderCode).HasMaxLength(9);
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.PaymentStatus).HasMaxLength(20);
                entity.Property(e => e.RegisteredUserEmail).HasMaxLength(256);
                entity.Property(e => e.RegisteredUserName).HasMaxLength(200);
                entity.Property(e => e.ShipCity).HasMaxLength(100);
                entity.Property(e => e.ShipCountry).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<VwProductList>(entity =>
            {
                entity.HasNoKey().ToView("vw_ProductList");
                entity.Property(e => e.AvgRating).HasColumnType("decimal(3, 1)");
                entity.Property(e => e.Badge).HasMaxLength(50);
                entity.Property(e => e.BadgeStyle).HasMaxLength(20);
                entity.Property(e => e.CategoryLabel).HasMaxLength(150);
                entity.Property(e => e.CategoryName).HasMaxLength(100);
                entity.Property(e => e.CategorySlug).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ShortDesc).HasMaxLength(500);
                entity.Property(e => e.Slug).HasMaxLength(200);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            });
            // ── WishlistItems ──────────────────────────────────────────────
            // Mỗi user chỉ có 1 lần yêu thích mỗi sản phẩm (unique constraint)
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                // Unique: mỗi cặp (UserId, ProductId) chỉ xuất hiện 1 lần
                entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Wishlist_User_Product").IsUnique();
                entity.HasIndex(e => e.UserId, "IX_WishlistItems_UserId");

                entity.Property(e => e.AddedAt).HasDefaultValueSql("(sysutcdatetime())");

                // FK -> AspNetUsers (cascade delete khi xóa user)
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WishlistItems_Users");

                // FK -> Products (set null khi xóa sản phẩm)
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WishlistItems_Products");
            });
        }
    }
}