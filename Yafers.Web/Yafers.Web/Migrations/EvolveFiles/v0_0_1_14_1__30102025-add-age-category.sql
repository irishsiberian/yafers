IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AgeCategories')
    BEGIN
        CREATE TABLE AgeCategories
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NULL,
            FeisId INT NOT NULL,
            CompetitionId INT NOT NULL,
            StartAge INT NOT NULL,
            EndAge INT NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_AgeCategories_Feiseanna FOREIGN KEY (FeisId) REFERENCES Feiseanna(Id),
            CONSTRAINT FK_AgeCategories_Competitions FOREIGN KEY (CompetitionId) REFERENCES Competitions(Id),
        );
    END


IF COL_LENGTH('dbo.Feiseanna', 'IsAssociationFeePaid') IS NULL
    BEGIN
        ALTER TABLE dbo.Feiseanna
            ADD IsAssociationFeePaid BIT NOT NULL DEFAULT(0);
    END
GO

IF COL_LENGTH('dbo.Feiseanna', 'AssociationFeePaidAtUtc') IS NULL
    BEGIN
        ALTER TABLE dbo.Feiseanna
            ADD AssociationFeePaidAtUtc DATETIME NULL;
    END
GO

-- === Удаляем колонки из Feiseanna, если они существуют ===
IF COL_LENGTH('dbo.Feiseanna', 'YafersFeePaidAtUtc') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.Feiseanna
            DROP COLUMN YafersFeePaidAtUtc;
    END
GO

IF COL_LENGTH('dbo.Feiseanna', 'IsCashPaymentsAllowed') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.Feiseanna
            DROP COLUMN IsCashPaymentsAllowed;
    END
GO


IF COL_LENGTH('dbo.DancerRegistrations', 'YafersFeePaidAtUtc') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD YafersFeePaidAtUtc DATETIME2 NULL;
    END
GO

IF COL_LENGTH('dbo.DancerRegistrations', 'IsCashPaymentsAllowed') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD IsCashPaymentsAllowed BIT NOT NULL DEFAULT(0);
    END
GO

IF COL_LENGTH('dbo.DancerRegistrations', 'IsInCart') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD IsInCart BIT NOT NULL DEFAULT(0);
    END

IF COL_LENGTH('dbo.CompetitionRegistrations', 'ModernSetSpeed') IS NULL
    BEGIN
        ALTER TABLE dbo.CompetitionRegistrations
            ADD ModernSetSpeed INT NULL;
    END
GO

IF COL_LENGTH('dbo.DancerRegistrations', 'InCartForUser') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD InCartForUser NVARCHAR(450) NULL;
    END
GO

IF COL_LENGTH('dbo.DancerRegistrations', 'AddedToCartAtUtc') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD AddedToCartAtUtc DATETIME2 NULL;
    END
GO


IF COL_LENGTH('dbo.CompetitionRegistrations', 'InCartForUser') IS NULL
    BEGIN
        ALTER TABLE dbo.CompetitionRegistrations
            ADD InCartForUser NVARCHAR(450) NULL;
    END
GO

IF COL_LENGTH('dbo.CompetitionRegistrations', 'AddedToCartAtUtc') IS NULL
    BEGIN
        ALTER TABLE dbo.CompetitionRegistrations
            ADD AddedToCartAtUtc DATETIME2 NULL;
    END
GO
/*
IF COL_LENGTH('dbo.DancerRegistrations', 'RegistrarId') IS NULL
    BEGIN
        ALTER TABLE dbo.DancerRegistrations
            ADD RegistrarId NVARCHAR(450) NOT NULL DEFAULT('');
    END
GO*/