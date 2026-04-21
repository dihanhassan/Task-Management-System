-- ============================================================
-- Task Management System — Database Schema
-- SQL Server
-- Run this script to set up the database from scratch.
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagementSystemDb')
BEGIN
    CREATE DATABASE TaskManagementSystemDb;
END
GO

USE TaskManagementSystemDb;
GO

-- ============================================================
-- Users
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
BEGIN
    CREATE TABLE [Users] (
        [Id]                   NVARCHAR(450)      NOT NULL,
        [UserName]             NVARCHAR(256)      NULL,
        [NormalizedUserName]   NVARCHAR(256)      NULL,
        [Email]                NVARCHAR(256)      NULL,
        [NormalizedEmail]      NVARCHAR(256)      NULL,
        [EmailConfirmed]       BIT                NOT NULL,
        [PasswordHash]         NVARCHAR(MAX)      NULL,
        [SecurityStamp]        NVARCHAR(MAX)      NULL,
        [ConcurrencyStamp]     NVARCHAR(MAX)      NULL,
        [PhoneNumber]          NVARCHAR(MAX)      NULL,
        [PhoneNumberConfirmed] BIT                NOT NULL,
        [TwoFactorEnabled]     BIT                NOT NULL,
        [LockoutEnd]           DATETIMEOFFSET     NULL,
        [LockoutEnabled]       BIT                NOT NULL,
        [AccessFailedCount]    INT                NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName])
        WHERE [NormalizedUserName] IS NOT NULL;

    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END
GO

-- ============================================================
-- Roles
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Roles' AND xtype = 'U')
BEGIN
    CREATE TABLE [Roles] (
        [Id]               NVARCHAR(450)  NOT NULL,
        [Name]             NVARCHAR(256)  NULL,
        [NormalizedName]   NVARCHAR(256)  NULL,
        [ConcurrencyStamp] NVARCHAR(MAX)  NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName])
        WHERE [NormalizedName] IS NOT NULL;
END
GO

-- ============================================================
-- UserRoles
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserRoles' AND xtype = 'U')
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_UserRoles]             PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END
GO

-- ============================================================
-- UserClaims
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserClaims' AND xtype = 'U')
BEGIN
    CREATE TABLE [UserClaims] (
        [Id]         INT           NOT NULL IDENTITY,
        [UserId]     NVARCHAR(450) NOT NULL,
        [ClaimType]  NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserClaims]              PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId]  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END
GO

-- ============================================================
-- UserLogins
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserLogins' AND xtype = 'U')
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider]       NVARCHAR(450) NOT NULL,
        [ProviderKey]         NVARCHAR(450) NOT NULL,
        [ProviderDisplayName] NVARCHAR(MAX) NULL,
        [UserId]              NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_UserLogins]              PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId]  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END
GO

-- ============================================================
-- UserTokens
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserTokens' AND xtype = 'U')
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId]        NVARCHAR(450) NOT NULL,
        [LoginProvider] NVARCHAR(450) NOT NULL,
        [Name]          NVARCHAR(450) NOT NULL,
        [Value]         NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserTokens]               PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId]   FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- RoleClaims
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'RoleClaims' AND xtype = 'U')
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id]         INT           NOT NULL IDENTITY,
        [RoleId]     NVARCHAR(450) NOT NULL,
        [ClaimType]  NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_RoleClaims]               PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId]   FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END
GO

-- ============================================================
-- Tasks
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Tasks' AND xtype = 'U')
BEGIN
    CREATE TABLE [Tasks] (
        [Id]          INT            NOT NULL IDENTITY(1,1),
        [Title]       NVARCHAR(100)  NOT NULL,
        [Description] NVARCHAR(4000) NULL,
        [DueDate]     DATETIME2      NULL,
        [Priority]    INT            NOT NULL,
        [IsCompleted] BIT            NOT NULL DEFAULT 0,
        [CreatedAt]   DATETIME2      NOT NULL DEFAULT (GETUTCDATE()),
        [UserId]      NVARCHAR(450)  NOT NULL,
        CONSTRAINT [PK_Tasks]              PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Tasks_Users_UserId]  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Tasks_UserId] ON [Tasks] ([UserId]);
END
GO

-- ============================================================
-- Priority reference:  1 = Low | 2 = Medium | 3 = High
-- ============================================================
