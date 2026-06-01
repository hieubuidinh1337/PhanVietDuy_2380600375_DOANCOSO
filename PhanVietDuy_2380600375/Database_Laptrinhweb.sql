-- ╔══════════════════════════════════════════════════════════════════════════╗
-- ║          VIETDUY LUXURY ATELIER — MS SQL SERVER DATABASE SCRIPT         ║
-- ║          Covers: Bài 1→6 (CRUD, EF Core, Identity, Cart, Order, API)    ║
-- ║          Target: SQL Server 2019+ / Azure SQL                            ║
-- ╚══════════════════════════════════════════════════════════════════════════╝

USE master;
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 0. TẠO DATABASE
-- ─────────────────────────────────────────────────────────────────────────────
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'VietduyDB')
BEGIN
    ALTER DATABASE VietduyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE VietduyDB;
END
GO

CREATE DATABASE VietduyDB
    COLLATE Vietnamese_CI_AS;   -- Hỗ trợ tiếng Việt
GO

USE VietduyDB;
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 1. ASP.NET CORE IDENTITY TABLES  (Bài 4 – Identity Core)
--    Đặt trước để Product/Order tham chiếu tới Users
-- ─────────────────────────────────────────────────────────────────────────────

-- 1.1 AspNetRoles
CREATE TABLE AspNetRoles (
    Id               NVARCHAR(450)  NOT NULL,
    Name             NVARCHAR(256)  NULL,
    NormalizedName   NVARCHAR(256)  NULL,
    ConcurrencyStamp NVARCHAR(MAX)  NULL,
    Description      NVARCHAR(500)  NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id),
    CONSTRAINT UQ_AspNetRoles_NormalizedName UNIQUE (NormalizedName)
);
GO

-- 1.2 AspNetUsers  (ApplicationUser mở rộng – Bài 4)
CREATE TABLE AspNetUsers (
    Id                   NVARCHAR(450)  NOT NULL,
    UserName             NVARCHAR(256)  NULL,
    NormalizedUserName   NVARCHAR(256)  NULL,
    Email                NVARCHAR(256)  NULL,
    NormalizedEmail      NVARCHAR(256)  NULL,
    EmailConfirmed       BIT            NOT NULL DEFAULT 0,
    PasswordHash         NVARCHAR(MAX)  NULL,
    SecurityStamp        NVARCHAR(MAX)  NULL,
    ConcurrencyStamp     NVARCHAR(MAX)  NULL,
    PhoneNumber          NVARCHAR(50)   NULL,
    PhoneNumberConfirmed BIT            NOT NULL DEFAULT 0,
    TwoFactorEnabled     BIT            NOT NULL DEFAULT 0,
    LockoutEnd           DATETIMEOFFSET NULL,
    LockoutEnabled       BIT            NOT NULL DEFAULT 1,
    AccessFailedCount    INT            NOT NULL DEFAULT 0,
    FullName             NVARCHAR(200)  NULL,
    Address              NVARCHAR(500)  NULL,
    City                 NVARCHAR(100)  NULL,
    Country              NVARCHAR(100)  NULL DEFAULT N'Vietnam',
    PostalCode           NVARCHAR(20)   NULL,
    Age                  TINYINT        NULL CHECK (Age >= 16 AND Age <= 120),
    AvatarUrl            NVARCHAR(1000) NULL,
    IsActive             BIT            NOT NULL DEFAULT 1,
    CreatedAt            DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    LastLoginAt          DATETIME2(7)   NULL,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id),
    CONSTRAINT UQ_AspNetUsers_NormalizedUserName UNIQUE (NormalizedUserName),
    CONSTRAINT UQ_AspNetUsers_NormalizedEmail    UNIQUE (NormalizedEmail)
);
GO

-- 1.3 AspNetUserRoles
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Roles FOREIGN KEY (RoleId)
        REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);
GO

-- 1.4 AspNetUserClaims
CREATE TABLE AspNetUserClaims (
    Id         INT            IDENTITY(1,1) NOT NULL,
    UserId     NVARCHAR(450)  NOT NULL,
    ClaimType  NVARCHAR(MAX)  NULL,
    ClaimValue NVARCHAR(MAX)  NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);
GO

-- 1.5 AspNetRoleClaims
CREATE TABLE AspNetRoleClaims (
    Id         INT            IDENTITY(1,1) NOT NULL,
    RoleId     NVARCHAR(450)  NOT NULL,
    ClaimType  NVARCHAR(MAX)  NULL,
    ClaimValue NVARCHAR(MAX)  NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetRoleClaims_Roles FOREIGN KEY (RoleId)
        REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);
GO

-- 1.6 AspNetUserLogins
CREATE TABLE AspNetUserLogins (
    LoginProvider       NVARCHAR(450) NOT NULL,
    ProviderKey         NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId              NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);
GO

-- 1.7 AspNetUserTokens
CREATE TABLE AspNetUserTokens (
    UserId        NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name          NVARCHAR(450) NOT NULL,
    Value         NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 2. DANH MỤC SẢN PHẨM  (Bài 2 – Category Model)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE Categories (
    Id          INT            IDENTITY(1,1) NOT NULL,
    Name        NVARCHAR(100)  NOT NULL,
    Slug        NVARCHAR(100)  NOT NULL,
    Label       NVARCHAR(150)  NOT NULL,
    Description NVARCHAR(1000) NULL,
    ImageUrl    NVARCHAR(1000) NULL,
    SortOrder   INT            NOT NULL DEFAULT 0,
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Categories PRIMARY KEY (Id),
    CONSTRAINT UQ_Categories_Slug UNIQUE (Slug)
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 3. SẢN PHẨM  (Bài 1 & 2 – Product Model + Data Annotations)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE Products (
    Id              INT             IDENTITY(1,1) NOT NULL,
    CategoryId      INT             NOT NULL,
    Name            NVARCHAR(200)   NOT NULL,
    Slug            NVARCHAR(200)   NOT NULL,
    ShortDesc       NVARCHAR(500)   NULL,
    Description     NVARCHAR(MAX)   NOT NULL,
    Materials       NVARCHAR(1000)  NULL,
    Dimensions      NVARCHAR(300)   NULL,
    ShippingInfo    NVARCHAR(1000)  NULL,
    Price           DECIMAL(18, 2)  NOT NULL CHECK (Price >= 0),
    OriginalPrice   DECIMAL(18, 2)  NULL,
    Badge           NVARCHAR(50)    NULL,
    BadgeStyle      NVARCHAR(20)    NULL DEFAULT 'default',
    IsFeatured      BIT             NOT NULL DEFAULT 0,
    IsActive        BIT             NOT NULL DEFAULT 1,
    StockQuantity   INT             NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    ThumbnailUrl    NVARCHAR(1000)  NULL,
    ThumbnailPath   NVARCHAR(1000)  NULL,
    CreatedAt       DATETIME2(7)    NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(7)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId NVARCHAR(450)   NULL,
    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT UQ_Products_Slug UNIQUE (Slug),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories (Id) ON DELETE NO ACTION,   -- SỬA: RESTRICT -> NO ACTION
    CONSTRAINT FK_Products_CreatedBy FOREIGN KEY (CreatedByUserId)
        REFERENCES AspNetUsers (Id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_Products_CategoryId   ON Products (CategoryId);
CREATE INDEX IX_Products_IsFeatured   ON Products (IsFeatured) WHERE IsFeatured = 1;
CREATE INDEX IX_Products_Price        ON Products (Price);
CREATE INDEX IX_Products_IsActive     ON Products (IsActive)   WHERE IsActive  = 1;
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 4. ẢNH SẢN PHẨM  (Bài 2 – upload nhiều hình ảnh)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE ProductImages (
    Id         INT             IDENTITY(1,1) NOT NULL,
    ProductId  INT             NOT NULL,
    Url        NVARCHAR(1000)  NOT NULL,
    FilePath   NVARCHAR(1000)  NULL,
    AltText    NVARCHAR(300)   NULL,
    SortOrder  TINYINT         NOT NULL DEFAULT 0,
    IsPrimary  BIT             NOT NULL DEFAULT 0,
    CreatedAt  DATETIME2(7)    NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_ProductImages PRIMARY KEY (Id),
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_ProductImages_ProductId ON ProductImages (ProductId);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 5. MÀU SẮC & SIZE  (options của product)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE ProductColors (
    Id        INT           IDENTITY(1,1) NOT NULL,
    ProductId INT           NOT NULL,
    ColorName NVARCHAR(100) NOT NULL,
    HexCode   NCHAR(7)      NULL,
    SortOrder TINYINT       NOT NULL DEFAULT 0,
    CONSTRAINT PK_ProductColors PRIMARY KEY (Id),
    CONSTRAINT FK_ProductColors_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE CASCADE
);
GO

CREATE TABLE ProductSizes (
    Id        INT           IDENTITY(1,1) NOT NULL,
    ProductId INT           NOT NULL,
    SizeName  NVARCHAR(50)  NOT NULL,
    SortOrder TINYINT       NOT NULL DEFAULT 0,
    CONSTRAINT PK_ProductSizes PRIMARY KEY (Id),
    CONSTRAINT FK_ProductSizes_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE CASCADE
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 6. ĐÁNH GIÁ SẢN PHẨM  (Review – mở rộng chức năng người dùng)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE Reviews (
    Id          INT            IDENTITY(1,1) NOT NULL,
    ProductId   INT            NOT NULL,
    UserId      NVARCHAR(450)  NULL,
    AuthorName  NVARCHAR(150)  NOT NULL,
    AuthorTitle NVARCHAR(200)  NULL,
    Rating      TINYINT        NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Content     NVARCHAR(2000) NOT NULL,
    IsApproved  BIT            NOT NULL DEFAULT 0,
    IsHomepage  BIT            NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Reviews PRIMARY KEY (Id),
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_Reviews_ProductId  ON Reviews (ProductId);
CREATE INDEX IX_Reviews_IsHomepage ON Reviews (IsHomepage) WHERE IsHomepage = 1;
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 7. ĐƠN HÀNG  (Bài 5 – Order Model)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE Orders (
    Id              INT            IDENTITY(1,1) NOT NULL,
    OrderCode       AS (N'VD-' + RIGHT(N'000000' + CAST(Id AS NVARCHAR(6)), 6)) PERSISTED,
    UserId          NVARCHAR(450)  NULL,
    CustomerName    NVARCHAR(200)  NOT NULL,
    CustomerEmail   NVARCHAR(256)  NOT NULL,
    CustomerPhone   NVARCHAR(30)   NOT NULL,
    ShipAddress     NVARCHAR(500)  NOT NULL,
    ShipCity        NVARCHAR(100)  NOT NULL,
    ShipCountry     NVARCHAR(100)  NOT NULL DEFAULT N'Vietnam',
    ShipPostalCode  NVARCHAR(20)   NULL,
    Note            NVARCHAR(1000) NULL,
    SubTotal        DECIMAL(18,2)  NOT NULL DEFAULT 0 CHECK (SubTotal >= 0),
    ShippingFee     DECIMAL(18,2)  NOT NULL DEFAULT 0 CHECK (ShippingFee >= 0),
    DiscountAmount  DECIMAL(18,2)  NOT NULL DEFAULT 0 CHECK (DiscountAmount >= 0),
    TotalAmount     DECIMAL(18,2)  NOT NULL DEFAULT 0 CHECK (TotalAmount >= 0),
    Status          NVARCHAR(30)   NOT NULL DEFAULT N'Pending'
                    CHECK (Status IN (N'Pending', N'Confirmed', N'Processing',
                                      N'Shipped', N'Delivered', N'Cancelled', N'Refunded')),
    PaymentMethod   NVARCHAR(50)   NOT NULL DEFAULT N'COD'
                    CHECK (PaymentMethod IN (N'COD', N'BankTransfer', N'Momo', N'VNPay', N'Stripe')),
    PaymentStatus   NVARCHAR(20)   NOT NULL DEFAULT N'Unpaid'
                    CHECK (PaymentStatus IN (N'Unpaid', N'Paid', N'Refunded')),
    OrderedAt       DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    ConfirmedAt     DATETIME2(7)   NULL,
    ShippedAt       DATETIME2(7)   NULL,
    DeliveredAt     DATETIME2(7)   NULL,
    CancelledAt     DATETIME2(7)   NULL,
    ProcessedByAdminId NVARCHAR(450) NULL,
    CONSTRAINT PK_Orders PRIMARY KEY (Id),
    CONSTRAINT FK_Orders_Users     FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE SET NULL,
    CONSTRAINT FK_Orders_AdminUser FOREIGN KEY (ProcessedByAdminId)
        REFERENCES AspNetUsers (Id)
);
GO

CREATE INDEX IX_Orders_UserId    ON Orders (UserId);
CREATE INDEX IX_Orders_Status    ON Orders (Status);
CREATE INDEX IX_Orders_OrderedAt ON Orders (OrderedAt DESC);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 8. CHI TIẾT ĐƠN HÀNG  (Bài 5 – OrderDetail Model)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE OrderDetails (
    Id          INT            IDENTITY(1,1) NOT NULL,
    OrderId     INT            NOT NULL,
    ProductId   INT            NULL,
    ProductName NVARCHAR(200)  NOT NULL,
    ProductSku  NVARCHAR(100)  NULL,
    ImageUrl    NVARCHAR(1000) NULL,
    UnitPrice   DECIMAL(18,2)  NOT NULL CHECK (UnitPrice >= 0),
    Quantity    INT            NOT NULL DEFAULT 1 CHECK (Quantity >= 1),
    LineTotal   AS (UnitPrice * Quantity) PERSISTED,
    SelectedColor NVARCHAR(100) NULL,
    SelectedSize  NVARCHAR(50)  NULL,
    CONSTRAINT PK_OrderDetails PRIMARY KEY (Id),
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_OrderDetails_OrderId   ON OrderDetails (OrderId);
CREATE INDEX IX_OrderDetails_ProductId ON OrderDetails (ProductId);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 9. LIÊN HỆ  (Contact page – Bài 1)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE ContactMessages (
    Id          INT            IDENTITY(1,1) NOT NULL,
    FullName    NVARCHAR(200)  NOT NULL,
    Email       NVARCHAR(256)  NOT NULL,
    Phone       NVARCHAR(30)   NULL,
    Subject     NVARCHAR(300)  NOT NULL,
    Message     NVARCHAR(MAX)  NOT NULL,
    IsRead      BIT            NOT NULL DEFAULT 0,
    IsReplied   BIT            NOT NULL DEFAULT 0,
    IpAddress   NVARCHAR(50)   NULL,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    RepliedAt   DATETIME2(7)   NULL,
    RepliedById NVARCHAR(450)  NULL,
    CONSTRAINT PK_ContactMessages PRIMARY KEY (Id),
    CONSTRAINT FK_ContactMessages_Admin FOREIGN KEY (RepliedById)
        REFERENCES AspNetUsers (Id) ON DELETE SET NULL
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 10. NEWSLETTER  (CTA Banner – trang chủ)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE NewsletterSubscribers (
    Id          INT            IDENTITY(1,1) NOT NULL,
    Email       NVARCHAR(256)  NOT NULL,
    FullName    NVARCHAR(200)  NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    SubscribedAt DATETIME2(7)  NOT NULL DEFAULT SYSUTCDATETIME(),
    UnsubscribedAt DATETIME2(7) NULL,
    ConfirmToken NVARCHAR(200) NULL,
    IsConfirmed  BIT           NOT NULL DEFAULT 0,
    CONSTRAINT PK_NewsletterSubscribers PRIMARY KEY (Id),
    CONSTRAINT UQ_NewsletterSubscribers_Email UNIQUE (Email)
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 11. COUPON / KHUYẾN MÃI  (mở rộng cart – Bài 5)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE Coupons (
    Id              INT            IDENTITY(1,1) NOT NULL,
    Code            NVARCHAR(50)   NOT NULL,
    Description     NVARCHAR(500)  NULL,
    DiscountType    NVARCHAR(20)   NOT NULL DEFAULT N'Percentage'
                    CHECK (DiscountType IN (N'Percentage', N'FixedAmount')),
    DiscountValue   DECIMAL(18,2)  NOT NULL CHECK (DiscountValue > 0),
    MinOrderAmount  DECIMAL(18,2)  NOT NULL DEFAULT 0,
    MaxUsage        INT            NULL,
    UsageCount      INT            NOT NULL DEFAULT 0,
    IsActive        BIT            NOT NULL DEFAULT 1,
    ExpiresAt       DATETIME2(7)   NULL,
    CreatedAt       DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Coupons PRIMARY KEY (Id),
    CONSTRAINT UQ_Coupons_Code UNIQUE (Code)
);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 12. GIỎ HÀNG BỀN VỮNG  (Bài 5 – Session + DB persistent cart)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE CartItems (
    Id          INT            IDENTITY(1,1) NOT NULL,
    SessionKey  NVARCHAR(100)  NOT NULL,
    UserId      NVARCHAR(450)  NULL,
    ProductId   INT            NOT NULL,
    Quantity    INT            NOT NULL DEFAULT 1 CHECK (Quantity >= 1),
    SelectedColor NVARCHAR(100) NULL,
    SelectedSize  NVARCHAR(50)  NULL,
    AddedAt     DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_CartItems PRIMARY KEY (Id),
    CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId)
        REFERENCES Products (Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_Users FOREIGN KEY (UserId)
        REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_CartItems_SessionKey ON CartItems (SessionKey);
CREATE INDEX IX_CartItems_UserId     ON CartItems (UserId);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- 13. API KEYS / AUDIT LOG  (Bài 6 – RESTful API)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE ApiLogs (
    Id          BIGINT         IDENTITY(1,1) NOT NULL,
    Endpoint    NVARCHAR(500)  NOT NULL,
    Method      NVARCHAR(10)   NOT NULL,
    StatusCode  SMALLINT       NOT NULL,
    RequestBody NVARCHAR(MAX)  NULL,
    UserId      NVARCHAR(450)  NULL,
    IpAddress   NVARCHAR(50)   NULL,
    DurationMs  INT            NULL,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_ApiLogs PRIMARY KEY (Id)
);
GO

CREATE INDEX IX_ApiLogs_CreatedAt ON ApiLogs (CreatedAt DESC);
CREATE INDEX IX_ApiLogs_Endpoint  ON ApiLogs (Endpoint);
GO


-- ╔══════════════════════════════════════════════════════════════════════════╗
-- ║                       SEED DATA — DỮ LIỆU MẪU                          ║
-- ╚══════════════════════════════════════════════════════════════════════════╝

-- ─────────────────────────────────────────────────────────────────────────────
-- S1. ROLES
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp, Description) VALUES
('role-admin-001',   N'Admin',   N'ADMIN',   NEWID(), N'Quản trị viên toàn hệ thống'),
('role-manager-001', N'Manager', N'MANAGER', NEWID(), N'Quản lý sản phẩm và đơn hàng'),
('role-staff-001',   N'Staff',   N'STAFF',   NEWID(), N'Nhân viên xem và xử lý đơn hàng'),
('role-user-001',    N'User',    N'USER',    NEWID(), N'Khách hàng đã đăng ký');
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S2. USERS
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO AspNetUsers (
    Id, UserName, NormalizedUserName, Email, NormalizedEmail,
    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
    PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
    LockoutEnabled, AccessFailedCount,
    FullName, Address, City, Country, Age, IsActive, CreatedAt
) VALUES
('user-admin-001', N'admin@vietduy.com', N'ADMIN@VIETDUY.COM',
 N'admin@vietduy.com', N'ADMIN@VIETDUY.COM',
 1, N'AQAAAAIAAYagAAAAEHashedAdminPassword1OfVietduy==',
 NEWID(), NEWID(),
 N'+84901234567', 1, 0, 1, 0,
 N'Nguyễn Việt Duy', N'12 Rue du Faubourg Saint-Honoré', N'Paris', N'France',
 35, 1, '2020-01-15'),

('user-manager-001', N'manager@vietduy.com', N'MANAGER@VIETDUY.COM',
 N'manager@vietduy.com', N'MANAGER@VIETDUY.COM',
 1, N'AQAAAAIAAYagAAAAEHashedManagerPassword==',
 NEWID(), NEWID(),
 N'+84912345678', 1, 0, 1, 0,
 N'Trần Thị Hương', N'45 Lê Lợi, Quận 1', N'Hồ Chí Minh', N'Vietnam',
 28, 1, '2021-03-10'),

('user-staff-001', N'staff01@vietduy.com', N'STAFF01@VIETDUY.COM',
 N'staff01@vietduy.com', N'STAFF01@VIETDUY.COM',
 1, N'AQAAAAIAAYagAAAAEHashedStaffPassword001==',
 NEWID(), NEWID(),
 N'+84923456789', 0, 0, 1, 0,
 N'Lê Minh Tuấn', N'88 Nguyễn Huệ, Quận 1', N'Hồ Chí Minh', N'Vietnam',
 24, 1, '2022-06-01'),

('user-cust-001', N'sontuong@example.com', N'SONTUONG@EXAMPLE.COM',
 N'sontuong@example.com', N'SONTUONG@EXAMPLE.COM',
 1, N'AQAAAAIAAYagAAAAEHashedUserPassword001==',
 NEWID(), NEWID(),
 N'+84934567890', 1, 0, 1, 0,
 N'Sơn Tùng MTP', N'123 Đường Láng, Đống Đa', N'Hà Nội', N'Vietnam',
 29, 1, '2023-02-14'),

('user-cust-002', N'anhthu@example.com', N'ANHTHU@EXAMPLE.COM',
 N'anhthu@example.com', N'ANHTHU@EXAMPLE.COM',
 1, N'AQAAAAIAAYagAAAAEHashedUserPassword002==',
 NEWID(), NEWID(),
 N'+84945678901', 0, 0, 1, 0,
 N'Nguyễn Ánh Thư', N'67 Trần Hưng Đạo, Quận 5', N'Hồ Chí Minh', N'Vietnam',
 33, 1, '2023-07-20'),

('user-cust-003', N'minhkhoa@example.com', N'MINHKHOA@EXAMPLE.COM',
 N'minhkhoa@example.com', N'MINHKHOA@EXAMPLE.COM',
 0, N'AQAAAAIAAYagAAAAEHashedUserPassword003==',
 NEWID(), NEWID(),
 N'+84956789012', 0, 0, 1, 0,
 N'Phạm Minh Khoa', N'15 Bùi Viện, Quận 1', N'Hồ Chí Minh', N'Vietnam',
 26, 1, '2024-01-05');
GO

-- Gán Roles
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
('user-admin-001',   'role-admin-001'),
('user-admin-001',   'role-user-001'),
('user-manager-001', 'role-manager-001'),
('user-manager-001', 'role-user-001'),
('user-staff-001',   'role-staff-001'),
('user-staff-001',   'role-user-001'),
('user-cust-001',    'role-user-001'),
('user-cust-002',    'role-user-001'),
('user-cust-003',    'role-user-001');
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S3. CATEGORIES
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO Categories (Name, Slug, Label, Description, ImageUrl, SortOrder) VALUES
(N'Túi Xách',        N'bags',         N'Bags & Leather Goods',
 N'Da cao cấp thủ công từ những xưởng da tốt nhất Italy và Pháp.',
 N'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=800&q=80', 1),

(N'Đồng Hồ',         N'timepieces',   N'Timepieces',
 N'Cơ chế Thụy Sĩ chính xác, thiết kế vượt thời gian.',
 N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800&q=80', 2),

(N'Lụa & Cashmere',  N'scarves',      N'Silk & Cashmere',
 N'Lụa Twill cao cấp và cashmere Mông Cổ Grade A.',
 N'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800&q=80', 3),

(N'Nước Hoa',        N'parfum',       N'Parfum',
 N'Hương thơm độc quyền từ nguyên liệu thiên nhiên quý hiếm.',
 N'https://images.unsplash.com/photo-1585386959984-a4155224a1ad?w=800&q=80', 4),

(N'Thời Trang',      N'ready-to-wear', N'Ready-to-Wear',
 N'Bộ sưu tập may mặc cao cấp từ những chất liệu tuyển chọn.',
 N'https://images.unsplash.com/photo-1611078489935-0cb964de46d6?w=800&q=80', 5),

(N'Trang Sức',       N'jewellery',    N'Fine Jewellery',
 N'Kim cương và đá quý được chế tác thủ công tinh xảo.',
 N'https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=800&q=80', 6);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S4. PRODUCTS (12 sản phẩm)
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO Products (
    CategoryId, Name, Slug, ShortDesc, Description,
    Materials, Dimensions, ShippingInfo,
    Price, OriginalPrice, Badge, BadgeStyle,
    IsFeatured, StockQuantity, ThumbnailUrl, CreatedByUserId
)
SELECT cat.Id, p.Name, p.Slug, p.ShortDesc, p.Description,
       p.Materials, p.Dimensions, p.Shipping,
       p.Price, p.OriginalPrice, p.Badge, p.BadgeStyle,
       p.Featured, p.Stock, p.ThumbnailUrl, 'user-admin-001'
FROM (VALUES
    (N'bags', N'Riviera Shoulder Bag', N'riviera-shoulder-bag',
     N'Da bê Italy thượng hạng, đường chỉ thủ công tỉ mỉ.',
     N'Riviera Shoulder Bag là đỉnh cao của nghệ thuật da thuộc Ý...',
     N'Full-grain Italian calfskin leather, khóa mạ vàng 24K, lót lụa Jacquard.',
     N'30 × 22 × 10 cm',
     N'Miễn phí vận chuyển toàn cầu trong 5–7 ngày làm việc.',
     2450.00, NULL, N'New', N'default', 1, 15,
     N'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=900&q=85'),

    (N'timepieces', N'Chronos Noir', N'chronos-noir',
     N'Cơ chế Thụy Sĩ 38 jewels, mặt đồng hồ men đen sâu.',
     N'Chronos Noir là sự giao thoa hoàn hảo giữa kỹ nghệ đồng hồ Thụy Sĩ...',
     N'Vỏ thép không gỉ 316L, dây da cá sấu Alligator, mặt kính sapphire.',
     N'42mm đường kính, độ dày 11.2mm',
     N'Giao hàng có bảo hiểm đặc biệt trong 3–5 ngày.',
     8900.00, NULL, NULL, N'default', 1, 8,
     N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=900&q=85'),

    (N'scarves', N'Jardin de Soir', N'jardin-de-soir',
     N'Lụa 100% Twill, hoạ tiết vẽ tay bởi nghệ nhân Florence.',
     N'Jardin de Soir là tuyệt phẩm từ lụa Twill 100% cao cấp...',
     N'100% Silk Twill Grade 6A, rìa cuộn tay thủ công.',
     N'90 × 90 cm',
     N'Giao hàng toàn cầu trong 5–7 ngày.',
     1200.00, 1500.00, N'Limited', N'gold', 0, 12,
     N'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=900&q=85'),

    (N'parfum', N'Oud Blossom No.7', N'oud-blossom-no7',
     N'Gỗ trầm Oud xứ Ả Rập hoà quyện hoa hồng Grasse.',
     N'Oud Blossom No.7 là tuyệt tác olfactory của Maison Vietduy...',
     N'Eau de Parfum 22%, 100ml. Chai pha lê cắt tay...',
     N'100ml | 50ml',
     N'Giao hàng cẩn thận trong 5–7 ngày.',
     680.00, NULL, NULL, N'default', 0, 30,
     N'https://images.unsplash.com/photo-1585386959984-a4155224a1ad?w=900&q=85'),

    (N'ready-to-wear', N'Manteau Cashmere', N'manteau-cashmere',
     N'Cashmere Mông Cổ Grade A, dáng oversize thời thượng.',
     N'Manteau Cashmere được dệt từ sợi cashmere Grade A Mông Cổ...',
     N'100% Grade A Mongolian Cashmere 2-ply, lót tơ Ý 100%.',
     N'XS / S / M / L / XL',
     N'Giao hàng quốc tế trong 5–7 ngày.',
     4300.00, NULL, NULL, N'default', 1, 20,
     N'https://images.unsplash.com/photo-1611078489935-0cb964de46d6?w=900&q=85'),

    (N'jewellery', N'Lumière Diamond Ring', N'lumiere-diamond-ring',
     N'Kim cương VVS1 2.4ct, vàng trắng 18K giát thủ công.',
     N'Lumière Diamond Ring là đỉnh cao của nghề kim hoàn Haute Joaillerie...',
     N'Kim cương VVS1 2.4ct màu D, vàng trắng 18K...',
     N'Sizes: 5, 5.5, 6, 6.5, 7, 7.5 (US Ring Size).',
     N'Giao hàng có bảo hiểm cao cấp Lloyds of London.',
     18500.00, NULL, N'Exclusive', N'gold', 1, 3,
     N'https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=900&q=85'),

    (N'bags', N'Portefeuille Élite', N'portefeuille-elite',
     N'Ví da đầu bò Pháp, ngăn đựng thẻ theo chuẩn RFID.',
     N'Portefeuille Élite là chiếc ví dành cho những người đàn ông sành điệu...',
     N'Full-grain French bull calf leather...',
     N'10 × 8.5 × 1.2 cm',
     N'Giao hàng trong 3–5 ngày.',
     580.00, NULL, NULL, N'default', 0, 25,
     N'https://images.unsplash.com/photo-1627123424574-724758594e93?w=900&q=85'),

    (N'jewellery', N'Belle Époque Necklace', N'belle-epoque-necklace',
     N'Vòng cổ vàng vàng 18K, ngọc trai Akoya Nhật Bản.',
     N'Belle Époque Necklace là sự hồi sinh của phong cách Art Nouveau...',
     N'Vàng vàng 18K, ngọc trai Akoya 8mm Grade AAA...',
     N'Dây dài 45cm (có thể điều chỉnh)',
     N'Giao hàng kèm hộp velvet cao cấp.',
     5800.00, NULL, NULL, N'default', 0, 7,
     N'https://images.unsplash.com/photo-1515562141207-7a88fb7ce338?w=900&q=85'),

    (N'parfum', N'Aqua Imperiale', N'aqua-imperiale',
     N'Hương biển Địa Trung Hải tinh khiết, mát lành và sang trọng.',
     N'Aqua Imperiale là ode của Maison Vietduy gửi tặng biển cả...',
     N'Eau de Toilette 15%, 100ml.',
     N'100ml',
     N'Giao hàng 5–7 ngày.',
     420.00, NULL, NULL, N'default', 0, 40,
     N'https://images.unsplash.com/photo-1563170351-be82bc888aa4?w=900&q=85'),

    (N'ready-to-wear', N'Veste Blazer Couture', N'veste-blazer-couture',
     N'Blazer len Italian Super 150s, dựng canvas thủ công.',
     N'Veste Blazer Couture là kiệt tác của nghề may tayloring Italy...',
     N'Wool Super 150s Italian, canvas horse hair & wool...',
     N'To measure (liên hệ để đặt may riêng).',
     N'Giao hàng 14–21 ngày (may to measure).',
     3200.00, NULL, NULL, N'default', 0, 10,
     N'https://images.unsplash.com/photo-1617137968427-85924c800a22?w=900&q=85'),

    (N'timepieces', N'Montre Squelette', N'montre-squelette',
     N'Đồng hồ cơ skeleton, máy hiển thị tourbillon thủ công.',
     N'Montre Squelette là tuyên ngôn của sự minh bạch cơ học...',
     N'Platinum 950 case, bộ máy tourbillon thủ công 21 jewels...',
     N'40mm, độ dày 9.8mm',
     N'Chỉ giao tận tay với nhân viên Vietduy.',
     24500.00, NULL, N'Exclusive', N'gold', 0, 2,
     N'https://images.unsplash.com/photo-1614164185128-e4ec99c436d7?w=900&q=85'),

    (N'scarves', N'Châle Cachemire', N'chale-cachemire',
     N'Khăn choàng cashmere 2-ply, họa tiết jacquard dệt tay.',
     N'Châle Cachemire là sự ấm áp sang trọng thuần túy...',
     N'100% Grade A Mongolian Cashmere 2-ply...',
     N'200 × 70 cm',
     N'Giao hàng trong 5–7 ngày.',
     890.00, 1100.00, NULL, N'default', 0, 18,
     N'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=900&q=85')
) AS p(CategorySlug, Name, Slug, ShortDesc, Description, Materials, Dimensions, Shipping,
       Price, OriginalPrice, Badge, BadgeStyle, Featured, Stock, ThumbnailUrl)
INNER JOIN Categories cat ON cat.Slug = p.CategorySlug;
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S5. PRODUCT IMAGES
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO ProductImages (ProductId, Url, AltText, SortOrder, IsPrimary) VALUES
(1, N'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=900&q=85', N'Riviera Bag - Front',   0, 1),
(1, N'https://images.unsplash.com/photo-1591561954557-26941169b49e?w=900&q=85', N'Riviera Bag - Side',    1, 0),
(1, N'https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=900&q=85', N'Riviera Bag - Detail',  2, 0),
(2, N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=900&q=85', N'Chronos Noir - Front',  0, 1),
(2, N'https://images.unsplash.com/photo-1614164185128-e4ec99c436d7?w=900&q=85', N'Chronos Noir - Caseback',1, 0),
(3, N'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=900&q=85', N'Jardin de Soir - Full',  0, 1),
(3, N'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=900&q=85', N'Jardin de Soir - Detail', 1, 0),
(4, N'https://images.unsplash.com/photo-1585386959984-a4155224a1ad?w=900&q=85', N'Oud Blossom - Bottle',   0, 1),
(4, N'https://images.unsplash.com/photo-1563170351-be82bc888aa4?w=900&q=85', N'Oud Blossom - Detail',   1, 0),
(5, N'https://images.unsplash.com/photo-1611078489935-0cb964de46d6?w=900&q=85', N'Manteau - Front',        0, 1),
(5, N'https://images.unsplash.com/photo-1539109136881-3be0616acf4b?w=900&q=85', N'Manteau - Styled',       1, 0),
(6, N'https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=900&q=85', N'Lumière Ring - Main',    0, 1),
(6, N'https://images.unsplash.com/photo-1515562141207-7a88fb7ce338?w=900&q=85', N'Lumière Ring - Side',    1, 0),
(7, N'https://images.unsplash.com/photo-1627123424574-724758594e93?w=900&q=85', N'Portefeuille Élite', 0, 1),
(8, N'https://images.unsplash.com/photo-1515562141207-7a88fb7ce338?w=900&q=85', N'Belle Époque Necklace', 0, 1),
(9, N'https://images.unsplash.com/photo-1563170351-be82bc888aa4?w=900&q=85', N'Aqua Imperiale', 0, 1),
(10,N'https://images.unsplash.com/photo-1617137968427-85924c800a22?w=900&q=85', N'Veste Blazer', 0, 1),
(11,N'https://images.unsplash.com/photo-1614164185128-e4ec99c436d7?w=900&q=85', N'Montre Squelette', 0, 1),
(12,N'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=900&q=85', N'Châle Cachemire', 0, 1);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S6. PRODUCT COLORS
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO ProductColors (ProductId, ColorName, HexCode, SortOrder) VALUES
(1, N'Noir',     N'#1A1814', 0),
(1, N'Camel',    N'#C19A6B', 1),
(1, N'Bordeaux', N'#722F37', 2),
(2, N'Noir / Steel', NULL, 0),
(2, N'Noir / Gold',  NULL, 1),
(3, N'Multicolor Original', NULL, 0),
(3, N'Marine & Or',         NULL, 1),
(4, N'Original',       NULL, 0),
(4, N'Édition Privée', NULL, 1),
(5, N'Camel',    N'#C19A6B', 0),
(5, N'Ivory',    N'#FFFFF0', 1),
(5, N'Charcoal', N'#36454F', 2),
(6, N'White Gold',  NULL, 0),
(6, N'Yellow Gold', NULL, 1),
(7, N'Noir', N'#1A1814', 0),
(7, N'Cognac', N'#9B4516', 1),
(8, N'Yellow Gold', NULL, 0),
(8, N'White Gold',  NULL, 1),
(9, N'Original', NULL, 0),
(10,N'Charcoal', N'#36454F', 0),
(10,N'Navy',     N'#1F305E', 1),
(11,N'Platinum / Brown', NULL, 0),
(12,N'Beige & Or',   NULL, 0),
(12,N'Navy & Ivory', NULL, 1);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S7. PRODUCT SIZES
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO ProductSizes (ProductId, SizeName, SortOrder) VALUES
(5, N'XS', 0), (5, N'S', 1), (5, N'M', 2), (5, N'L', 3), (5, N'XL', 4),
(4, N'50ml', 0), (4, N'100ml', 1),
(9, N'100ml', 0),
(6, N'5', 0), (6, N'5.5', 1), (6, N'6', 2), (6, N'6.5', 3), (6, N'7', 4), (6, N'7.5', 5),
(10,N'46', 0), (10,N'48', 1), (10,N'50', 2), (10,N'52', 3), (10,N'54', 4);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S8. REVIEWS
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO Reviews (ProductId, UserId, AuthorName, AuthorTitle, Rating, Content, IsApproved, IsHomepage)
VALUES
(1, 'user-cust-001', N'Sơn Tùng MTP', N'Ca sĩ top 1 Việt Nam', 5,
 N'Mỗi lần cầm chiếc túi Riviera trên tay, tôi biết mình đang giữ một tác phẩm nghệ thuật.', 1, 1),
(2, 'user-cust-002', N'Nguyễn Ánh Thư', N'Fashion Director, L''Officiel Vietnam', 5,
 N'Chronos Noir là chiếc đồng hồ tôi mặc mỗi ngày.', 1, 0),
(6, 'user-cust-001', N'Sơn Tùng MTP', N'Ca sĩ', 5,
 N'Tặng cho người thân nhân dịp sinh nhật. Chiếc nhẫn đẹp hơn tôi tưởng.', 1, 0),
(5, 'user-cust-002', N'Nguyễn Ánh Thư', N'Fashion Director', 5,
 N'Manteau Cashmere của Vietduy là một trong những khoản đầu tư thời trang tốt nhất.', 1, 0),
(3, NULL, N'Phạm Lan Anh', N'Khách hàng tại Hà Nội', 4,
 N'Jardin de Soir đẹp hơn rất nhiều so với ảnh trên website.', 1, 0);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S9. COUPONS (đã sửa lỗi thiếu cột ExpiresAt)
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO Coupons (Code, Description, DiscountType, DiscountValue, MinOrderAmount, MaxUsage, IsActive, ExpiresAt)
VALUES
(N'WELCOME10',  N'Giảm 10% cho đơn hàng đầu tiên',      N'Percentage',  10.00,  500.00, 500, 1, '2025-12-31'),
(N'VIP20',      N'Giảm 20% dành riêng cho VIP members',  N'Percentage',  20.00, 2000.00, 100, 1, '2025-09-30'),
(N'SUMMER300',  N'Giảm $300 khi mua từ $3000',           N'FixedAmount', 300.00, 3000.00, 200, 1, '2025-08-31'),
(N'FREESHIP',   N'Miễn phí vận chuyển toàn cầu',         N'FixedAmount',  50.00,    0.00, NULL, 1, NULL),
(N'NOIR2025',   N'Bộ sưu tập Noir: Giảm 15%',            N'Percentage',  15.00, 1000.00,  50, 1, '2025-07-15');
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S10. ORDERS + ORDER DETAILS
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO Orders (UserId, CustomerName, CustomerEmail, CustomerPhone,
    ShipAddress, ShipCity, ShipCountry,
    SubTotal, ShippingFee, DiscountAmount, TotalAmount,
    Status, PaymentMethod, PaymentStatus,
    OrderedAt, ConfirmedAt, ShippedAt, DeliveredAt, ProcessedByAdminId)
VALUES
('user-cust-001', N'Sơn Tùng MTP', N'sontuong@example.com', N'+84934567890',
 N'123 Đường Láng, Đống Đa', N'Hà Nội', N'Vietnam',
 11350.00, 0.00, 1135.00, 10215.00,
 N'Delivered', N'BankTransfer', N'Paid',
 '2025-02-10 09:15:00', '2025-02-10 11:30:00', '2025-02-12 08:00:00', '2025-02-17 14:30:00',
 'user-admin-001');

INSERT INTO OrderDetails (OrderId, ProductId, ProductName, ProductSku, ImageUrl, UnitPrice, Quantity, SelectedColor, SelectedSize)
VALUES
(1, 1, N'Riviera Shoulder Bag', N'VD-BAG-001', N'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=900&q=85', 2450.00, 1, N'Noir', NULL),
(1, 6, N'Lumière Diamond Ring', N'VD-JWL-006', N'https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=900&q=85', 18500.00, 1, N'White Gold', N'6');

INSERT INTO Orders (UserId, CustomerName, CustomerEmail, CustomerPhone,
    ShipAddress, ShipCity, ShipCountry,
    SubTotal, ShippingFee, DiscountAmount, TotalAmount,
    Status, PaymentMethod, PaymentStatus,
    OrderedAt, ConfirmedAt, ProcessedByAdminId)
VALUES
('user-cust-002', N'Nguyễn Ánh Thư', N'anhthu@example.com', N'+84945678901',
 N'67 Trần Hưng Đạo, Quận 5', N'Hồ Chí Minh', N'Vietnam',
 13200.00, 0.00, 0.00, 13200.00,
 N'Confirmed', N'Momo', N'Paid',
 '2025-05-01 14:22:00', '2025-05-01 16:00:00',
 'user-staff-001');

INSERT INTO OrderDetails (OrderId, ProductId, ProductName, ProductSku, ImageUrl, UnitPrice, Quantity, SelectedColor, SelectedSize)
VALUES
(2, 2, N'Chronos Noir', N'VD-WATCH-002', N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=900&q=85', 8900.00, 1, N'Noir / Steel', NULL),
(2, 5, N'Manteau Cashmere', N'VD-RTW-005', N'https://images.unsplash.com/photo-1611078489935-0cb964de46d6?w=900&q=85', 4300.00, 1, N'Ivory', N'S');

INSERT INTO Orders (UserId, CustomerName, CustomerEmail, CustomerPhone,
    ShipAddress, ShipCity, ShipCountry, Note,
    SubTotal, ShippingFee, DiscountAmount, TotalAmount,
    Status, PaymentMethod, PaymentStatus, OrderedAt)
VALUES
(NULL, N'Trần Văn Bình', N'binh.tran@gmail.com', N'+84967890123',
 N'45 Nguyễn Trãi, Thanh Xuân', N'Hà Nội', N'Vietnam',
 N'Vui lòng đóng gói hộp quà và ghi thiệp: "Happy Birthday"',
 1880.00, 50.00, 188.00, 1742.00,
 N'Pending', N'COD', N'Unpaid', '2025-05-15 20:45:00');

INSERT INTO OrderDetails (OrderId, ProductId, ProductName, ProductSku, ImageUrl, UnitPrice, Quantity, SelectedColor, SelectedSize)
VALUES
(3, 3, N'Jardin de Soir', N'VD-SLK-003', N'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=900&q=85', 1200.00, 1, N'Marine & Or', NULL),
(3, 4, N'Oud Blossom No.7', N'VD-PRF-004', N'https://images.unsplash.com/photo-1585386959984-a4155224a1ad?w=900&q=85', 680.00, 1, N'Original', N'100ml');

INSERT INTO Orders (UserId, CustomerName, CustomerEmail, CustomerPhone,
    ShipAddress, ShipCity, ShipCountry,
    SubTotal, ShippingFee, DiscountAmount, TotalAmount,
    Status, PaymentMethod, PaymentStatus, OrderedAt, ConfirmedAt)
VALUES
('user-cust-003', N'Phạm Minh Khoa', N'minhkhoa@example.com', N'+84956789012',
 N'15 Bùi Viện, Quận 1', N'Hồ Chí Minh', N'Vietnam',
 9480.00, 0.00, 0.00, 9480.00,
 N'Processing', N'VNPay', N'Paid',
 '2025-05-14 11:00:00', '2025-05-14 12:30:00', 'user-manager-001');

INSERT INTO OrderDetails (OrderId, ProductId, ProductName, ProductSku, ImageUrl, UnitPrice, Quantity, SelectedColor, SelectedSize)
VALUES
(4, 5, N'Manteau Cashmere', N'VD-RTW-005', N'https://images.unsplash.com/photo-1611078489935-0cb964de46d6?w=900&q=85', 4300.00, 2, N'Camel', N'M'),
(4, 7, N'Portefeuille Élite', N'VD-BAG-007', N'https://images.unsplash.com/photo-1627123424574-724758594e93?w=900&q=85', 580.00, 1, N'Cognac', NULL);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S11. CONTACT MESSAGES
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO ContactMessages (FullName, Email, Phone, Subject, Message, IsRead)
VALUES
(N'Lê Hoàng Nam', N'hoangnam@example.com', N'+84901112233',
 N'Hỏi về dịch vụ may đo riêng',
 N'Xin chào team Vietduy. Tôi muốn hỏi về dịch vụ may đo riêng cho áo blazer.',
 0),
(N'Ngô Thị Lan', N'ngtlan@example.com', NULL,
 N'Muốn mua làm quà cưới',
 N'Tôi đang tìm một món quà cưới thật đặc biệt cho bạn thân.',
 1);
GO

-- ─────────────────────────────────────────────────────────────────────────────
-- S12. NEWSLETTER
-- ─────────────────────────────────────────────────────────────────────────────
INSERT INTO NewsletterSubscribers (Email, FullName, IsConfirmed)
VALUES
(N'fanvietduy01@gmail.com', N'Nguyễn Thành Đạt', 1),
(N'luxurylover@yahoo.com', NULL, 1),
(N'style.vn@outlook.com', N'Phạm Thu Hà', 0),
(N'sonnguyen.fashion@gmail.com', N'Sơn Nguyễn', 1),
(N'anhthu@example.com', N'Nguyễn Ánh Thư', 1);
GO

-- ╔══════════════════════════════════════════════════════════════════════════╗
-- ║                    STORED PROCEDURES & VIEWS                            ║
-- ╚══════════════════════════════════════════════════════════════════════════╝

-- V1. VIEW: Danh sách sản phẩm đầy đủ
CREATE OR ALTER VIEW vw_ProductList AS
SELECT
    p.Id,
    p.Name,
    p.Slug,
    p.ShortDesc,
    p.Price,
    p.OriginalPrice,
    CASE WHEN p.OriginalPrice IS NOT NULL AND p.OriginalPrice > p.Price
         THEN CAST(ROUND((1 - p.Price / p.OriginalPrice) * 100, 0) AS INT)
         ELSE NULL END AS DiscountPct,
    p.Badge,
    p.BadgeStyle,
    p.IsFeatured,
    p.StockQuantity,
    p.IsActive,
    p.ThumbnailUrl,
    p.CreatedAt,
    p.UpdatedAt,
    c.Id AS CategoryId,
    c.Name AS CategoryName,
    c.Slug AS CategorySlug,
    c.Label AS CategoryLabel,
    ISNULL(r.AvgRating, 0) AS AvgRating,
    ISNULL(r.TotalReviews, 0) AS TotalReviews,
    ISNULL(img.ImageCount, 0) AS ImageCount
FROM Products p
INNER JOIN Categories c ON c.Id = p.CategoryId
LEFT JOIN (
    SELECT ProductId,
           CAST(AVG(CAST(Rating AS FLOAT)) AS DECIMAL(3,1)) AS AvgRating,
           COUNT(*) AS TotalReviews
    FROM Reviews WHERE IsApproved = 1
    GROUP BY ProductId
) r ON r.ProductId = p.Id
LEFT JOIN (
    SELECT ProductId, COUNT(*) AS ImageCount
    FROM ProductImages GROUP BY ProductId
) img ON img.ProductId = p.Id;
GO

-- V2. VIEW: Đơn hàng đầy đủ
CREATE OR ALTER VIEW vw_OrderSummary AS
SELECT
    o.Id,
    o.OrderCode,
    o.CustomerName,
    o.CustomerEmail,
    o.CustomerPhone,
    o.ShipCity,
    o.ShipCountry,
    o.TotalAmount,
    o.Status,
    o.PaymentMethod,
    o.PaymentStatus,
    o.OrderedAt,
    o.DeliveredAt,
    u.FullName AS RegisteredUserName,
    u.Email AS RegisteredUserEmail,
    d.ItemCount,
    d.TotalQty
FROM Orders o
LEFT JOIN AspNetUsers u ON u.Id = o.UserId
LEFT JOIN (
    SELECT OrderId, COUNT(*) AS ItemCount, SUM(Quantity) AS TotalQty
    FROM OrderDetails GROUP BY OrderId
) d ON d.OrderId = o.Id;
GO

-- SP1. Stored Procedure: Tìm kiếm & lọc sản phẩm
CREATE OR ALTER PROCEDURE sp_SearchProducts
    @CategorySlug  NVARCHAR(100) = NULL,
    @SearchText    NVARCHAR(200) = NULL,
    @MinPrice      DECIMAL(18,2) = NULL,
    @MaxPrice      DECIMAL(18,2) = NULL,
    @SortBy        NVARCHAR(30)  = 'default',
    @PageNumber    INT           = 1,
    @PageSize      INT           = 12
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *, COUNT(*) OVER() AS TotalCount
    FROM vw_ProductList
    WHERE IsActive = 1
      AND (@CategorySlug IS NULL OR CategorySlug = @CategorySlug)
      AND (@SearchText   IS NULL OR Name LIKE N'%' + @SearchText + N'%'
                                 OR ShortDesc LIKE N'%' + @SearchText + N'%')
      AND (@MinPrice IS NULL OR Price >= @MinPrice)
      AND (@MaxPrice IS NULL OR Price <= @MaxPrice)
    ORDER BY
        CASE WHEN @SortBy = 'price_asc'  THEN Price       END ASC,
        CASE WHEN @SortBy = 'price_desc' THEN Price       END DESC,
        CASE WHEN @SortBy = 'rating'     THEN AvgRating   END DESC,
        CASE WHEN @SortBy = 'newest'     THEN CreatedAt   END DESC,
        IsFeatured DESC, CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- SP2. Tạo đơn hàng từ session cart
CREATE OR ALTER PROCEDURE sp_CreateOrder
    @UserId          NVARCHAR(450),
    @CustomerName    NVARCHAR(200),
    @CustomerEmail   NVARCHAR(256),
    @CustomerPhone   NVARCHAR(30),
    @ShipAddress     NVARCHAR(500),
    @ShipCity        NVARCHAR(100),
    @ShipCountry     NVARCHAR(100),
    @Note            NVARCHAR(1000),
    @PaymentMethod   NVARCHAR(50),
    @CouponCode      NVARCHAR(50) = NULL,
    @CartXml         XML
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @CartItems TABLE (
            ProductId INT, Quantity INT,
            SelectedColor NVARCHAR(100), SelectedSize NVARCHAR(50)
        );
        INSERT INTO @CartItems
        SELECT
            item.value('@productId', 'INT'),
            item.value('@qty',       'INT'),
            item.value('@color',     'NVARCHAR(100)'),
            item.value('@size',      'NVARCHAR(50)')
        FROM @CartXml.nodes('/cart/item') AS x(item);

        DECLARE @SubTotal DECIMAL(18,2);
        SELECT @SubTotal = SUM(p.Price * ci.Quantity)
        FROM @CartItems ci
        INNER JOIN Products p ON p.Id = ci.ProductId;

        DECLARE @Discount DECIMAL(18,2) = 0;
        IF @CouponCode IS NOT NULL
        BEGIN
            SELECT @Discount =
                CASE DiscountType
                    WHEN 'Percentage'  THEN @SubTotal * DiscountValue / 100
                    WHEN 'FixedAmount' THEN DiscountValue
                END
            FROM Coupons
            WHERE Code = @CouponCode AND IsActive = 1
              AND (ExpiresAt IS NULL OR ExpiresAt > SYSUTCDATETIME())
              AND @SubTotal >= MinOrderAmount;
            IF @Discount IS NOT NULL
                UPDATE Coupons SET UsageCount += 1 WHERE Code = @CouponCode;
            ELSE
                SET @Discount = 0;
        END

        DECLARE @ShipFee DECIMAL(18,2) = CASE WHEN @SubTotal >= 1000 THEN 0 ELSE 50 END;
        DECLARE @Total   DECIMAL(18,2) = @SubTotal - @Discount + @ShipFee;

        INSERT INTO Orders (UserId, CustomerName, CustomerEmail, CustomerPhone,
            ShipAddress, ShipCity, ShipCountry, Note,
            SubTotal, ShippingFee, DiscountAmount, TotalAmount,
            Status, PaymentMethod, PaymentStatus)
        VALUES (@UserId, @CustomerName, @CustomerEmail, @CustomerPhone,
            @ShipAddress, @ShipCity, @ShipCountry, @Note,
            @SubTotal, @ShipFee, @Discount, @Total,
            N'Pending', @PaymentMethod, N'Unpaid');

        DECLARE @NewOrderId INT = SCOPE_IDENTITY();

        INSERT INTO OrderDetails (OrderId, ProductId, ProductName, ImageUrl, UnitPrice, Quantity, SelectedColor, SelectedSize)
        SELECT @NewOrderId, p.Id, p.Name, p.ThumbnailUrl, p.Price, ci.Quantity, ci.SelectedColor, ci.SelectedSize
        FROM @CartItems ci
        INNER JOIN Products p ON p.Id = ci.ProductId;

        UPDATE p SET p.StockQuantity -= ci.Quantity
        FROM Products p
        INNER JOIN @CartItems ci ON ci.ProductId = p.Id;

        COMMIT TRANSACTION;
        SELECT @NewOrderId AS NewOrderId, N'Success' AS Result;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT -1 AS NewOrderId, ERROR_MESSAGE() AS Result;
    END CATCH;
END;
GO

-- SP3. Dashboard thống kê
CREATE OR ALTER PROCEDURE sp_AdminDashboard
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*)          FROM Products     WHERE IsActive = 1)    AS TotalProducts,
        (SELECT COUNT(*)          FROM Categories   WHERE IsActive = 1)    AS TotalCategories,
        (SELECT COUNT(*)          FROM Orders)                              AS TotalOrders,
        (SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE Status = N'Delivered') AS TotalRevenue,
        (SELECT COUNT(*)          FROM Orders WHERE Status = N'Pending')   AS PendingOrders,
        (SELECT COUNT(*)          FROM AspNetUsers WHERE IsActive = 1)     AS TotalUsers,
        (SELECT COUNT(*)          FROM Reviews WHERE IsApproved = 0)       AS PendingReviews,
        (SELECT COUNT(*)          FROM ContactMessages WHERE IsRead = 0)   AS UnreadMessages;

    SELECT
        FORMAT(OrderedAt, 'yyyy-MM') AS YearMonth,
        COUNT(*)                     AS OrderCount,
        SUM(TotalAmount)             AS Revenue
    FROM Orders
    WHERE Status NOT IN (N'Cancelled', N'Refunded')
      AND OrderedAt >= DATEADD(MONTH, -6, SYSUTCDATETIME())
    GROUP BY FORMAT(OrderedAt, 'yyyy-MM')
    ORDER BY YearMonth;

    SELECT TOP 5
        p.Name,
        SUM(od.Quantity)      AS TotalSold,
        SUM(od.LineTotal)     AS TotalRevenue
    FROM OrderDetails od
    INNER JOIN Products p ON p.Id = od.ProductId
    INNER JOIN Orders   o ON o.Id = od.OrderId
    WHERE o.Status NOT IN (N'Cancelled', N'Refunded')
    GROUP BY p.Id, p.Name
    ORDER BY TotalSold DESC;
END;
GO

-- INDEXES bổ sung cho performance
CREATE INDEX IX_Products_Slug        ON Products (Slug) INCLUDE (Name, Price, ThumbnailUrl, IsActive);
CREATE INDEX IX_Categories_Slug      ON Categories (Slug) INCLUDE (Name, Label);
CREATE INDEX IX_Reviews_ProductApprv ON Reviews (ProductId, IsApproved) INCLUDE (Rating, AuthorName, Content);
GO

-- Kiểm tra dữ liệu
PRINT N'';
PRINT N'════════════════════════════════════════════';
PRINT N'  VIETDUY DATABASE — SEED VERIFICATION';
PRINT N'════════════════════════════════════════════';
PRINT N'';

SELECT N'AspNetRoles'            AS [Table], COUNT(*) AS [Rows] FROM AspNetRoles            UNION ALL
SELECT N'AspNetUsers',                        COUNT(*)           FROM AspNetUsers             UNION ALL
SELECT N'AspNetUserRoles',                    COUNT(*)           FROM AspNetUserRoles         UNION ALL
SELECT N'Categories',                         COUNT(*)           FROM Categories              UNION ALL
SELECT N'Products',                           COUNT(*)           FROM Products                UNION ALL
SELECT N'ProductImages',                      COUNT(*)           FROM ProductImages           UNION ALL
SELECT N'ProductColors',                      COUNT(*)           FROM ProductColors           UNION ALL
SELECT N'ProductSizes',                       COUNT(*)           FROM ProductSizes            UNION ALL
SELECT N'Reviews',                            COUNT(*)           FROM Reviews                 UNION ALL
SELECT N'Orders',                             COUNT(*)           FROM Orders                  UNION ALL
SELECT N'OrderDetails',                       COUNT(*)           FROM OrderDetails            UNION ALL
SELECT N'Coupons',                            COUNT(*)           FROM Coupons                 UNION ALL
SELECT N'ContactMessages',                    COUNT(*)           FROM ContactMessages         UNION ALL
SELECT N'NewsletterSubscribers',              COUNT(*)           FROM NewsletterSubscribers;

PRINT N'';
PRINT N'  Test: sp_SearchProducts (Bags category)';
EXEC sp_SearchProducts @CategorySlug = N'bags';

PRINT N'';
PRINT N'  Test: sp_AdminDashboard';
EXEC sp_AdminDashboard;

PRINT N'';
PRINT N'  ✓ VietduyDB created & seeded successfully!';
GO