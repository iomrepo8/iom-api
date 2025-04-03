CREATE TABLE [dbo].[AspNetRoles] (
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    [RoleCode] VARCHAR(5) NULL, 
    [PermissionsLocked] BIT NOT NULL DEFAULT 0, 
    [IsAllUsers] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([Name] ASC);

