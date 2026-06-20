-- 1. ავტომატურად შექმნის ბაზას, თუ ის ჯერ არ არსებობს სერვერზე
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BackpackStoreDB')
BEGIN
    CREATE DATABASE BackpackStoreDB;
END
GO

USE BackpackStoreDB;
GO

-- 2. იწყება Entity Framework-ის მიგრაციის ორიგინალი სტრუქტურა
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] int NOT NULL IDENTITY,
    [role] nvarchar(max) NOT NULL,
    [created_at] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NOT NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [AK_AspNetUsers_Email] UNIQUE ([Email])
);
GO

CREATE TABLE [Categories] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ShippingAddress] nvarchar(500) NOT NULL,
    [TrackingNumber] nvarchar(100) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PasswordResetCodes] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(256) NOT NULL,
    [Code] nvarchar(max) NOT NULL,
    [ExpiryTime] datetime2 NOT NULL,
    CONSTRAINT [PK_PasswordResetCodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PasswordResetCodes_AspNetUsers_Email] FOREIGN KEY ([Email]) REFERENCES [AspNetUsers] ([Email]) ON DELETE CASCADE
);
GO

CREATE TABLE [Backpacks] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(max) NOT NULL,
    [price] decimal(5,2) NOT NULL,
    [quantity] int NOT NULL,
    [sale_price] decimal(5,2) NULL,
    [CategoryId] int NOT NULL,
    [IsNew] bit NOT NULL,
    CONSTRAINT [PK_Backpacks] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Backpacks_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BackpackImages] (
    [Id] int NOT NULL IDENTITY,
    [Url] nvarchar(max) NOT NULL,
    [BackpackId] int NOT NULL,
    CONSTRAINT [PK_BackpackImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BackpackImages_Backpacks_BackpackId] FOREIGN KEY ([BackpackId]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BasketsItems] (
    [id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [BackpackId] int NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_BasketsItems] PRIMARY KEY ([id]),
    CONSTRAINT [FK_BasketsItems_Backpacks_BackpackId] FOREIGN KEY ([BackpackId]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderItem] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [BackpackId] int NOT NULL,
    [Quantity] int NOT NULL,
    [PriceAtPurchase] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItem_Backpacks_BackpackId] FOREIGN KEY ([BackpackId]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItem_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Ratings] (
    [Id] int NOT NULL IDENTITY,
    [Value] int NOT NULL,
    [UserId] int NOT NULL,
    [BackpackId] int NOT NULL,
    CONSTRAINT [PK_Ratings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ratings_Backpacks_BackpackId] FOREIGN KEY ([BackpackId]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Reviews] (
    [Id] int NOT NULL IDENTITY,
    [Comment] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [BackpackId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Backpacks_BackpackId] FOREIGN KEY ([BackpackId]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [WishlistItems] (
    [id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [backpack_id] int NOT NULL,
    CONSTRAINT [PK_WishlistItems] PRIMARY KEY ([id]),
    CONSTRAINT [FK_WishlistItems_AspNetUsers_user_id] FOREIGN KEY ([user_id]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WishlistItems_Backpacks_backpack_id] FOREIGN KEY ([backpack_id]) REFERENCES [Backpacks] ([id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([id], [name])
VALUES (1, N'Backpacks'),
(2, N'Duffel Bags'),
(3, N'Travel Packs');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'CategoryId', N'IsNew', N'name', N'price', N'quantity', N'sale_price') AND [object_id] = OBJECT_ID(N'[Backpacks]'))
    SET IDENTITY_INSERT [Backpacks] ON;
INSERT INTO [Backpacks] ([id], [CategoryId], [IsNew], [name], [price], [quantity], [sale_price])
VALUES (1, 1, CAST(0 AS bit), N'APEX COMMUTER', 90.0, 10, 65.0),
(2, 1, CAST(1 AS bit), N'APEX HERITAGE', 75.0, 12, NULL),
(3, 1, CAST(1 AS bit), N'APEX PULSE', 80.0, 8, NULL),
(4, 1, CAST(0 AS bit), N'APEX STEALTH', 110.0, 6, 95.0),
(5, 1, CAST(0 AS bit), N'APEX SKYLINE', 95.0, 9, NULL),
(6, 1, CAST(0 AS bit), N'APEX GLOBAL', 120.0, 5, 85.0),
(7, 2, CAST(0 AS bit), N'APEX CROSSOVER', 95.0, 7, NULL),
(8, 2, CAST(0 AS bit), N'APEX EXECUTIVE', 150.0, 4, 115.0),
(9, 2, CAST(1 AS bit), N'APEX IGNITE', 85.0, 11, NULL),
(10, 2, CAST(0 AS bit), N'APEX TRANSFORMER', 110.0, 6, 89.0),
(11, 2, CAST(0 AS bit), N'APEX LEGACY', 85.0, 10, NULL),
(12, 3, CAST(0 AS bit), N'APEX ODYSSEY', 160.0, 3, 130.0),
(13, 3, CAST(1 AS bit), N'APEX VOYAGER', 145.0, 5, NULL),
(14, 3, CAST(0 AS bit), N'APEX SUMMIT', 130.0, 4, NULL),
(15, 3, CAST(0 AS bit), N'APEX CYBER', 180.0, 2, 149.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'CategoryId', N'IsNew', N'name', N'price', N'quantity', N'sale_price') AND [object_id] = OBJECT_ID(N'[Backpacks]'))
    SET IDENTITY_INSERT [Backpacks] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BackpackId', N'Url') AND [object_id] = OBJECT_ID(N'[BackpackImages]'))
    SET IDENTITY_INSERT [BackpackImages] ON;
INSERT INTO [BackpackImages] ([Id], [BackpackId], [Url])
VALUES (1, 1, N'/images/APEX_Commuter.png'),
(2, 2, N'/images/APEX_Heritage.png'),
(3, 3, N'/images/APEX_Pulse.png'),
(4, 4, N'/images/APEX_Stealth.png'),
(5, 5, N'/images/APEX_Skyline.png'),
(6, 6, N'/images/APEX_Global.png'),
(7, 7, N'/images/APEX_Crossover.png'),
(8, 8, N'/images/APEX_Executive.png'),
(9, 9, N'/images/APEX_Ignite.png'),
(10, 10, N'/images/APEX_Transformer.png'),
(11, 11, N'/images/APEX_Legacy.png'),
(12, 12, N'/images/APEX_Odyssey.png'),
(13, 13, N'/images/APEX_Voyager.png'),
(14, 14, N'/images/APEX_Summit.png'),
(15, 15, N'/images/APEX_Cyber.png');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BackpackId', N'Url') AND [object_id] = OBJECT_ID(N'[BackpackImages]'))
    SET IDENTITY_INSERT [BackpackImages] OFF;
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO
CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO
CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO
CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO
CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO
CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO
CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO
CREATE INDEX [IX_BackpackImages_BackpackId] ON [BackpackImages] ([BackpackId]);
GO
CREATE INDEX [IX_Backpacks_CategoryId] ON [Backpacks] ([CategoryId]);
GO
CREATE INDEX [IX_BasketsItems_BackpackId] ON [BasketsItems] ([BackpackId]);
GO
CREATE INDEX [IX_OrderItem_BackpackId] ON [OrderItem] ([BackpackId]);
GO
CREATE INDEX [IX_OrderItem_OrderId] ON [OrderItem] ([OrderId]);
GO
CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
GO
CREATE INDEX [IX_PasswordResetCodes_Email] ON [PasswordResetCodes] ([Email]);
GO
CREATE INDEX [IX_Ratings_BackpackId] ON [Ratings] ([BackpackId]);
GO
CREATE INDEX [IX_Reviews_BackpackId] ON [Reviews] ([BackpackId]);
GO
CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);
GO
CREATE INDEX [IX_WishlistItems_backpack_id] ON [WishlistItems] ([backpack_id]);
GO
CREATE INDEX [IX_WishlistItems_user_id] ON [WishlistItems] ([user_id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402143814_InitialCreate', N'8.0.11');
GO
COMMIT;
GO

BEGIN TRANSACTION;
GO
ALTER TABLE [Backpacks] ADD [description] nvarchar(1000) NOT NULL DEFAULT N'';
GO

UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX COMMUTER combines minimalist design with maximum durability.' WHERE [id] = 1;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX HERITAGE combines minimalist design with maximum durability.' WHERE [id] = 2;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX PULSE combines minimalist design with maximum durability.' WHERE [id] = 3;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX STEALTH combines minimalist design with maximum durability.' WHERE [id] = 4;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX SKYLINE combines minimalist design with maximum durability.' WHERE [id] = 5;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX GLOBAL combines minimalist design with maximum durability.' WHERE [id] = 6;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX CROSSOVER combines minimalist design with maximum durability.' WHERE [id] = 7;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX EXECUTIVE combines minimalist design with maximum durability.' WHERE [id] = 8;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX IGNITE combines minimalist design with maximum durability.' WHERE [id] = 9;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX TRANSFORMER combines minimalist design with maximum durability.' WHERE [id] = 10;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX LEGACY combines minimalist design with maximum durability.' WHERE [id] = 11;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX ODYSSEY combines minimalist design with maximum durability.' WHERE [id] = 12;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX VOYAGER combines minimalist design with maximum durability.' WHERE [id] = 13;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX SUMMIT combines minimalist design with maximum durability.' WHERE [id] = 14;
UPDATE [Backpacks] SET [description] = N'Engineered for the modern explorer, the APEX CYBER combines minimalist design with maximum durability.' WHERE [id] = 15;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260412234331_AddDescriptionProperty', N'8.0.11');
GO
COMMIT;
GO