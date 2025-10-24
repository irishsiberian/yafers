IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Audit')
    BEGIN
        CREATE TABLE Audit
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            CreatedDate DATETIME2 NULL,
            TableName NVARCHAR(256) NULL,
            Changes NVARCHAR(MAX) NULL,
            LastModifiedById INT NULL,
            AuditableObjectId INT NULL,
            AuditableEntityType INT NULL
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Associations')
    BEGIN
        CREATE TABLE Associations
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ceilis')
    BEGIN
        CREATE TABLE Ceilis
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            MinTeamSize INT NOT NULL,
            MaxTeamSize INT NOT NULL
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ModernSets')
    BEGIN
        CREATE TABLE ModernSets
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            Type INT NOT NULL  -- enum хранится как INT
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Dances')
    BEGIN
        CREATE TABLE Dances
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DancerParents')
    BEGIN
        CREATE TABLE DancerParents
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UserId NVARCHAR(450) NOT NULL, -- FK на AspNetUsers
            FirstName NVARCHAR(100) NOT NULL,
            LastName NVARCHAR(100) NOT NULL,
            Phone NVARCHAR(50) NULL,
            Email NVARCHAR(100) NULL,
            DancerIds NVARCHAR(MAX) NULL, -- хранит Id танцоров, если нужно

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_DancerParents_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Schools')
    BEGIN
        CREATE TABLE Schools
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            Country NVARCHAR(100) NOT NULL,
            City NVARCHAR(100) NOT NULL,
            Address NVARCHAR(200) NOT NULL,
            AssociationId INT NOT NULL,
            ParentId INT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Schools_Associations FOREIGN KEY (AssociationId) REFERENCES Associations(Id),
            CONSTRAINT FK_Schools_Parent FOREIGN KEY (ParentId) REFERENCES Schools(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Teams')
    BEGIN
        CREATE TABLE Teams
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            Description NVARCHAR(MAX) NULL
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Organisers')
    BEGIN
        CREATE TABLE Organisers
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UserId NVARCHAR(450) NOT NULL,           -- FK на AspNetUsers
            StripeEnabled BIT NOT NULL DEFAULT(0),
            StripeKey NVARCHAR(200) NULL,
            PayPalEnabled BIT NOT NULL DEFAULT(0),
            PayPalCode NVARCHAR(MAX) NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Organisers_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Teachers')
    BEGIN
        CREATE TABLE Teachers
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            FirstName NVARCHAR(100) NOT NULL,
            LastName NVARCHAR(100) NOT NULL,
            SchoolId INT NOT NULL,
            Phone NVARCHAR(50) NULL,
            Email NVARCHAR(100) NULL,
            UserId NVARCHAR(450) NULL,
            Qualification INT NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Teachers_Schools FOREIGN KEY (SchoolId) REFERENCES Schools(Id),
            CONSTRAINT FK_Teachers_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Syllabi')
    BEGIN
        CREATE TABLE Syllabi
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            AssociationId INT NOT NULL,
            AdminFee DECIMAL(18,2) NOT NULL,
            IsTemplate BIT NOT NULL DEFAULT(0),
            SoloDancePrice DECIMAL(18,2) NOT NULL,
            PremiershipPrice DECIMAL(18,2) NOT NULL,
            ChampionshipPrice DECIMAL(18,2) NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Syllabi_Associations FOREIGN KEY (AssociationId) REFERENCES Associations(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Competitions')
    BEGIN
        CREATE TABLE Competitions
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            DanceId INT NULL, -- can be null for special dance
            Level INT NOT NULL,           -- enum DanceLevel
            Speed INT NOT NULL,
            MinAge INT NOT NULL,
            MaxAge INT NOT NULL,
            IsTeam BIT NOT NULL DEFAULT(0),
            TeamType INT NOT NULL,        -- enum TeamType
            IsModernSet BIT NOT NULL DEFAULT(0),
            IsSpecial BIT NOT NULL DEFAULT(0),
            Description NVARCHAR(MAX) NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Competitions_Dances FOREIGN KEY (DanceId) REFERENCES Dances(Id),
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Feisanna')
    BEGIN
        CREATE TABLE Feisanna
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            FeisDate DATETIME2 NOT NULL,
            RegistrationOpenDate DATETIME2 NOT NULL,
            RegistrationCloseDate DATETIME2 NOT NULL,
            Location NVARCHAR(1024) NOT NULL,
            Venue NVARCHAR(1024) NOT NULL,
            OrganiserSchoolId INT NOT NULL,     -- FK на Schools
            Contacts NVARCHAR(1024) NOT NULL,
            EventUrl NVARCHAR(4096) NULL,
            Description NVARCHAR(MAX) NULL,
            FeisType INT NOT NULL,              -- enum FeisType
            AssociationId INT NOT NULL,         -- FK на Associations
            SyllabusId INT NOT NULL,            -- FK на Syllabi
            MaxEntriesCount INT NULL,
            Status INT NOT NULL,                -- enum FeisStatus

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Feisanna_Schools FOREIGN KEY (OrganiserSchoolId) REFERENCES Schools(Id),
            CONSTRAINT FK_Feisanna_Associations FOREIGN KEY (AssociationId) REFERENCES Associations(Id),
            CONSTRAINT FK_Feisanna_Syllabi FOREIGN KEY (SyllabusId) REFERENCES Syllabi(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Invoices')
    BEGIN
        CREATE TABLE Invoices
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            FeisId INT NOT NULL,                 -- FK на Feis
            InvoicedAtUtc DATETIME2 NOT NULL,
            PaidAtUtc DATETIME2 NULL,
            PaidBy NVARCHAR(450) NULL,
            TotalSum DECIMAL(18,2) NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Invoices_Feis FOREIGN KEY (FeisId) REFERENCES Feisanna(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Dancers')
    BEGIN
        CREATE TABLE Dancers
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            FirstName NVARCHAR(100) NOT NULL,
            LastName NVARCHAR(100) NOT NULL,
            BirthDate DATETIME2 NOT NULL,
            Gender INT NOT NULL,                -- enum Gender
            SchoolId INT NOT NULL,              -- FK на Schools
            UserId NVARCHAR(450) NULL,          -- FK на AspNetUsers
            DancerParentId INT NULL,            -- FK на DancerParents
            DancerParentUserId NVARCHAR(450) NULL, -- FK на AspNetUsers
            PreliminaryWinCount INT NOT NULL DEFAULT(0),

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Dancers_Schools FOREIGN KEY (SchoolId) REFERENCES Schools(Id),
            CONSTRAINT FK_Dancers_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
            CONSTRAINT FK_Dancers_DancerParents FOREIGN KEY (DancerParentId) REFERENCES DancerParents(Id),
            CONSTRAINT FK_Dancers_DancerParentUsers FOREIGN KEY (DancerParentUserId) REFERENCES AspNetUsers(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DancerLevels')
    BEGIN
        CREATE TABLE DancerLevels
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            DancerId INT NOT NULL,
            DanceId INT NOT NULL,
            Level INT NOT NULL, -- enum хранится как INT

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_DancerLevels_Dancers FOREIGN KEY (DancerId) REFERENCES Dancers(Id),
            CONSTRAINT FK_DancerLevels_Dances FOREIGN KEY (DanceId) REFERENCES Dances(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DancerRegistrations')
    BEGIN
        CREATE TABLE DancerRegistrations
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            DancerId INT NOT NULL,            -- FK на Dancers
            FeisId INT NOT NULL,              -- FK на Feisanna
            DancerNumber INT NOT NULL,
            NumberAssignedAtUtc DATETIME2 NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_DancerRegistrations_Dancers FOREIGN KEY (DancerId) REFERENCES Dancers(Id),
            CONSTRAINT FK_DancerRegistrations_Feis FOREIGN KEY (FeisId) REFERENCES Feisanna(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CompetitionRegistrations')
    BEGIN
        CREATE TABLE CompetitionRegistrations
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            DancerId INT NOT NULL,           -- FK на Dancers
            DancerRegistrationId INT NOT NULL, -- FK на DancerRegistrations
            FeisId INT NOT NULL,             -- FK на Feisanna
            CompetitionId INT NOT NULL,      -- FK на Competitions
            InvoiceId INT NULL,              -- FK на Invoices
            TeamId INT NULL,                 -- FK на Teams
            ModernSetId INT NULL,            -- FK на ModernSets
            CeiliId INT NULL,                -- FK на Ceilis
            RegistrarId NVARCHAR(450) NULL,  -- FK на AspNetUsers
            IsInCart BIT NOT NULL DEFAULT(0),

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_CompetitionRegistrations_DancerRegistrations FOREIGN KEY (DancerRegistrationId) REFERENCES DancerRegistrations(Id),
            CONSTRAINT FK_CompetitionRegistrations_Dancers FOREIGN KEY (DancerId) REFERENCES Dancers(Id),
            CONSTRAINT FK_CompetitionRegistrations_Feis FOREIGN KEY (FeisId) REFERENCES Feisanna(Id),
            CONSTRAINT FK_CompetitionRegistrations_Competitions FOREIGN KEY (CompetitionId) REFERENCES Competitions(Id),
            CONSTRAINT FK_CompetitionRegistrations_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id),
            CONSTRAINT FK_CompetitionRegistrations_Teams FOREIGN KEY (TeamId) REFERENCES Teams(Id),
            CONSTRAINT FK_CompetitionRegistrations_ModernSets FOREIGN KEY (ModernSetId) REFERENCES ModernSets(Id),
            CONSTRAINT FK_CompetitionRegistrations_Ceilis FOREIGN KEY (CeiliId) REFERENCES Ceilis(Id),
            CONSTRAINT FK_CompetitionRegistrations_Users FOREIGN KEY (RegistrarId) REFERENCES AspNetUsers(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Reports')
    BEGIN
        CREATE TABLE Reports
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            FeisId INT NOT NULL,                -- FK на Feisanna
            FileUrl NVARCHAR(4096) NOT NULL,
            FileName NVARCHAR(1024) NOT NULL,
            Type INT NOT NULL,                   -- enum ReportType
            Status INT NOT NULL,                 -- enum ReportStatus
            CreatedAtUtc DATETIME2 NOT NULL,

            CONSTRAINT FK_Reports_Feis FOREIGN KEY (FeisId) REFERENCES Feisanna(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rounds')
    BEGIN
        CREATE TABLE Rounds
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Name NVARCHAR(200) NOT NULL,
            RoundOrder INT NOT NULL,
            DanceId INT NOT NULL,           -- FK на Dances
            Description NVARCHAR(MAX) NULL,
            CompetitionId INT NOT NULL,     -- FK на Competitions

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_Rounds_Dances FOREIGN KEY (DanceId) REFERENCES Dances(Id),
            CONSTRAINT FK_Rounds_Competitions FOREIGN KEY (CompetitionId) REFERENCES Competitions(Id)
        );
    END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SyllabusCompetitions')
    BEGIN
        CREATE TABLE SyllabusCompetitions
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            CompetitionId INT NOT NULL,      -- FK на Competitions
            SyllabusId INT NOT NULL,         -- FK на Syllabi
            PriceOverride DECIMAL(18,2) NOT NULL,
            RegistrationOrder INT NOT NULL,

            CreatedAtUtc DATETIME2 NOT NULL,
            CreatedBy NVARCHAR(450) NOT NULL,
            UpdatedAtUtc DATETIME2 NULL,
            UpdatedBy NVARCHAR(450) NULL,
            DeletedAtUtc DATETIME2 NULL,
            DeletedBy NVARCHAR(450) NULL,
            IsDeleted BIT NOT NULL DEFAULT(0),

            CONSTRAINT FK_SyllabusCompetitions_Competitions FOREIGN KEY (CompetitionId) REFERENCES Competitions(Id),
            CONSTRAINT FK_SyllabusCompetitions_Syllabi FOREIGN KEY (SyllabusId) REFERENCES Syllabi(Id)
        );
    END
