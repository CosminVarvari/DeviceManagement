USE DeviceManagementDB;
GO

-- Seed: Users
-- Passwords are hashed with BCrypt for "Password123!"
-- These are only used for seed data.
-- Actual passwords will be generated and stored via the Register API.
MERGE INTO Users AS target
USING (VALUES
    (
        'A1000000-0000-0000-0000-000000000001',
        'Admin User',
        'admin@company.com',
        '$2a$11$zL3J1bQk5V8mN2pX9wR4.uKtYhGfDcEiOaLmSnVbWxQzJrPkMuHye',
        'Admin',
        'HQ - Cluj-Napoca'
    ),
    (
        'A2000000-0000-0000-0000-000000000002',
        'Ion Popescu',
        'ion.popescu@company.com',
        '$2a$11$zL3J1bQk5V8mN2pX9wR4.uKtYhGfDcEiOaLmSnVbWxQzJrPkMuHye',
        'User',
        'Office - București'
    ),
    (
        'A3000000-0000-0000-0000-000000000003',
        'Maria Ionescu',
        'maria.ionescu@company.com',
        '$2a$11$zL3J1bQk5V8mN2pX9wR4.uKtYhGfDcEiOaLmSnVbWxQzJrPkMuHye',
        'User',
        'Remote - Timișoara'
    ),
    (
        'A4000000-0000-0000-0000-000000000004',
        'Andrei Constantin',
        'andrei.constantin@company.com',
        '$2a$11$zL3J1bQk5V8mN2pX9wR4.uKtYhGfDcEiOaLmSnVbWxQzJrPkMuHye',
        'User',
        'Office - Cluj-Napoca'
    )
) AS source (Id, Name, Email, PasswordHash, Role, Location)
ON target.Email = source.Email 
WHEN NOT MATCHED THEN
    INSERT (Id, Name, Email, PasswordHash, Role, Location)
    VALUES (source.Id, source.Name, source.Email, source.PasswordHash, source.Role, source.Location);

PRINT 'Users seeded.';
GO

-- Seed: Devices
MERGE INTO Devices AS target
USING (VALUES
    (
        'B1000000-0000-0000-0000-000000000001',
        'iPhone 15 Pro', 'Apple', 'Phone', 'iOS', '17.4',
        'A17 Pro', 8,
        'High-performance Apple smartphone for daily business use.',
        'A2000000-0000-0000-0000-000000000002'
    ),
    (
        'B2000000-0000-0000-0000-000000000002',
        'Galaxy S24 Ultra', 'Samsung', 'Phone', 'Android', '14.0',
        'Snapdragon 8 Gen 3', 12,
        'Flagship Samsung phone with S Pen support.',
        'A3000000-0000-0000-0000-000000000003'
    ),
    (
        'B3000000-0000-0000-0000-000000000003',
        'Pixel 8 Pro', 'Google', 'Phone', 'Android', '14.0',
        'Google Tensor G3', 12,
        'Google flagship with best-in-class camera system.',
        'A4000000-0000-0000-0000-000000000004'
    ),
    (
        'B4000000-0000-0000-0000-000000000004',
        'iPhone 14', 'Apple', 'Phone', 'iOS', '17.2',
        'A15 Bionic', 6,
        'Reliable Apple smartphone available for assignment.',
        NULL
    ),
    (
        'B5000000-0000-0000-0000-000000000005',
        'OnePlus 12', 'OnePlus', 'Phone', 'Android', '14.0',
        'Snapdragon 8 Gen 3', 16,
        'High RAM phone suitable for heavy multitasking.',
        NULL
    ),
    (
        'B6000000-0000-0000-0000-000000000006',
        'iPad Pro 12.9', 'Apple', 'Tablet', 'iPadOS', '17.4',
        'M2', 16,
        'Professional Apple tablet for creative and office work.',
        'A2000000-0000-0000-0000-000000000002'
    ),
    (
        'B7000000-0000-0000-0000-000000000007',
        'Galaxy Tab S9 Ultra', 'Samsung', 'Tablet', 'Android', '14.0',
        'Snapdragon 8 Gen 2', 12,
        'Large Samsung tablet ideal for presentations and multitasking.',
        'A3000000-0000-0000-0000-000000000003'
    ),
    (
        'B8000000-0000-0000-0000-000000000008',
        'iPad Air 5', 'Apple', 'Tablet', 'iPadOS', '17.2',
        'M1', 8,
        'Lightweight Apple tablet available for assignment.',
        NULL
    ),
    (
        'B9000000-0000-0000-0000-000000000009',
        'Lenovo Tab P12 Pro', 'Lenovo', 'Tablet', 'Android', '13.0',
        'Snapdragon 870', 8,
        'Affordable business tablet with large display.',
        NULL
    ),
    (
        'BA000000-0000-0000-0000-00000000000A',
        'Surface Pro 9', 'Microsoft', 'Tablet', 'Windows', '11',
        'Intel Core i7', 16,
        'Windows tablet with full desktop capabilities.',
        NULL
    )
) AS source (
    Id, Name, Manufacturer, Type, OperatingSystem, OsVersion,
    Processor, RamAmount, Description, AssignedUserId
)
ON target.Name = source.Name AND target.Manufacturer = source.Manufacturer
WHEN NOT MATCHED THEN
    INSERT (
        Id, Name, Manufacturer, Type, OperatingSystem, OsVersion,
        Processor, RamAmount, Description, AssignedUserId
    )
    VALUES (
        source.Id, source.Name, source.Manufacturer, source.Type,
        source.OperatingSystem, source.OsVersion, source.Processor,
        source.RamAmount, source.Description, source.AssignedUserId
    );

PRINT 'Devices seeded.';
GO