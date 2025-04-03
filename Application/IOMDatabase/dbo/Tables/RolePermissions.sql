CREATE TABLE [dbo].[RolePermissions]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RoleId] NVARCHAR(128) NOT NULL, 
    [ModuleId] INT NOT NULL, 
    [IsLocked] BIT NOT NULL DEFAULT 0, 
    [CanView] BIT NOT NULL, 
    [CanAdd] BIT NOT NULL, 
    [CanEdit] BIT NOT NULL, 
    [CanDelete] BIT NOT NULL, 
    [CreatedBy] VARCHAR(150) NULL, 
    [DateCreated] DATETIME NULL, 
    [ModuleCode] NVARCHAR(128) NOT NULL, 
    CONSTRAINT [FK_RolePermissions_ToAspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]), 
    CONSTRAINT [FK_RolePermissions_AspNetModules] FOREIGN KEY (ModuleId) REFERENCES [AspNetModules]([Id]) 
)
