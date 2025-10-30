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


