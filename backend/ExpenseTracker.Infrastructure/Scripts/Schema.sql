CREATE DATABASE ExpenseTracker;
GO

USE ExpenseTracker;
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Expenses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Amount DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Date DATETIME2 NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Stored Procedures
CREATE PROCEDURE sp_CreateUser
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(MAX),
    @NewId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewId = NEWID();
    INSERT INTO Users (Id, Email, PasswordHash)
    VALUES (@NewId, @Email, @PasswordHash);
END
GO

CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Email, PasswordHash, CreatedAt 
    FROM Users 
    WHERE Email = @Email;
END
GO

CREATE PROCEDURE sp_CreateExpense
    @UserId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Date DATETIME2,
    @Notes NVARCHAR(MAX),
    @NewId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewId = NEWID();
    INSERT INTO Expenses (Id, UserId, Amount, Category, Date, Notes)
    VALUES (@NewId, @UserId, @Amount, @Category, @Date, @Notes);
END
GO

CREATE PROCEDURE sp_GetExpensesByUserId
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Amount, Category, Date, Notes, CreatedAt, UpdatedAt
    FROM Expenses
    WHERE UserId = @UserId
    ORDER BY Date DESC;
END
GO

CREATE PROCEDURE sp_UpdateExpense
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Date DATETIME2,
    @Notes NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Expenses
    SET Amount = @Amount,
        Category = @Category,
        Date = @Date,
        Notes = @Notes,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND UserId = @UserId;
END
GO

CREATE PROCEDURE sp_DeleteExpense
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Expenses WHERE Id = @Id AND UserId = @UserId;
END
GO
