IF NOT EXISTS (
    SELECT name FROM sys.databases WHERE name = 'DeviceManagementDB'
)
BEGIN
    CREATE DATABASE DeviceManagementDB;
    PRINT 'Database DeviceManagementDB created.';
END
ELSE
BEGIN
    PRINT 'Database DeviceManagementDB already exists. Skipping.';
END
GO

USE DeviceManagementDB;
GO

-- Table: Users
IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'Users'
)
BEGIN
    CREATE TABLE Users (
        Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        Name            NVARCHAR(100)       NOT NULL,
        Email           NVARCHAR(200)       NOT NULL,
        PasswordHash    NVARCHAR(MAX)       NOT NULL,
        Role            NVARCHAR(20)        NOT NULL DEFAULT 'User',
        Location        NVARCHAR(200)       NULL,
        CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT PK_Users PRIMARY KEY (Id),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'User'))
    );
    PRINT 'Table Users created.';
END
ELSE
BEGIN
    PRINT 'Table Users already exists. Skipping.';
END
GO

-- Table: Devices
IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'Devices'
)
BEGIN
    CREATE TABLE Devices (
        Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
        Name            NVARCHAR(150)       NOT NULL,
        Manufacturer    NVARCHAR(100)       NOT NULL,
        Type            NVARCHAR(20)        NOT NULL,
        OperatingSystem NVARCHAR(50)        NOT NULL,
        OsVersion       NVARCHAR(50)        NOT NULL,
        Processor       NVARCHAR(100)       NOT NULL,
        RamAmount       INT                 NOT NULL,
        Description     NVARCHAR(1000)      NULL,
        AssignedUserId  UNIQUEIDENTIFIER    NULL,
        CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT PK_Devices PRIMARY KEY (Id),
        CONSTRAINT FK_Devices_Users FOREIGN KEY (AssignedUserId)
            REFERENCES Users(Id) ON DELETE SET NULL,
        CONSTRAINT CK_Devices_Type CHECK (Type IN ('Phone', 'Tablet')),
        CONSTRAINT CK_Devices_Ram CHECK (RamAmount > 0),
        CONSTRAINT UQ_Devices_Name_Manufacturer UNIQUE (Name, Manufacturer)
    );
    PRINT 'Table Devices created.';
END
ELSE
BEGIN
    PRINT 'Table Devices already exists. Skipping.';
END
GO