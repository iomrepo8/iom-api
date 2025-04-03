
IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserPermission'))
BEGIN
	--The following statement was imported into the database project as a schema object and named dbo.AspNetUserPermission.
--CREATE TABLE [dbo].[AspNetUserPermission](
--		[id] [int] IDENTITY(1,1) NOT NULL,
--		[UserId] [nvarchar](128) NULL,
--		[Module_ID] [nvarchar](128) NULL,
--		[CanView] [bit] NULL,
--		[CanEdit] [bit] NULL,
--		[CanDelete] [bit] NULL,
--		[CanAdd] [bit] NULL,
--		[CreatedBy] [nvarchar](max) NULL,
--		[DateCreated] [datetime] NULL,
--		[UpdateBy] [nvarchar](max) NULL,
--		[DateUpdated] [datetime] NULL,
--		[IsActive] [bit] NULL,
--	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
--	(
--		[id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserAccounts'))
BEGIN
    --The following statement was imported into the database project as a schema object and named dbo.AspNetUserAccounts.
--CREATE TABLE [dbo].[AspNetUserAccounts](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Account_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserAccounts] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserBaseUsers'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserBaseUsers.
--CREATE TABLE [dbo].[AspNetUserBaseUsers](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[UserBase_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserBaseUsers] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserTeams'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserTeams.
--CREATE TABLE [dbo].[AspNetUserTeams](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Team_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserTeams] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]


END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserPermission'))
BEGIN
	--The following statement was imported into the database project as a schema object and named dbo.AspNetUserPermission.
--CREATE TABLE [dbo].[AspNetUserPermission](
--		[id] [int] IDENTITY(1,1) NOT NULL,
--		[UserId] [nvarchar](128) NULL,
--		[Module_ID] [nvarchar](128) NULL,
--		[CanView] [bit] NULL,
--		[CanEdit] [bit] NULL,
--		[CanDelete] [bit] NULL,
--		[CanAdd] [bit] NULL,
--		[CreatedBy] [nvarchar](max) NULL,
--		[DateCreated] [datetime] NULL,
--		[UpdateBy] [nvarchar](max) NULL,
--		[DateUpdated] [datetime] NULL,
--		[IsActive] [bit] NULL,
--	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
--	(
--		[id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserAccounts'))
BEGIN
    --The following statement was imported into the database project as a schema object and named dbo.AspNetUserAccounts.
--CREATE TABLE [dbo].[AspNetUserAccounts](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Account_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserAccounts] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserBaseUsers'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserBaseUsers.
--CREATE TABLE [dbo].[AspNetUserBaseUsers](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[UserBase_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserBaseUsers] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserTeams'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserTeams.
--CREATE TABLE [dbo].[AspNetUserTeams](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Team_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserTeams] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]


END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetModules'))
BEGIN
--The following statement was imported into the database project as a schema object and named dbo.AspNetModules.
--CREATE TABLE [dbo].[AspNetModules](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[Module_ID] [nvarchar](max) NULL,
--	[Module_Name] [nvarchar](max) NULL,
--	[CreatedBy] [nvarchar](max) NULL,
--	[DateCreated] [datetime] NULL,
--	[UpdateBy] [nvarchar](max) NULL,
--	[DateUpdated] [datetime] NULL,
--	[IsActive] [bit] NULL
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserPermission'))
BEGIN
	--The following statement was imported into the database project as a schema object and named dbo.AspNetUserPermission.
--CREATE TABLE [dbo].[AspNetUserPermission](
--		[id] [int] IDENTITY(1,1) NOT NULL,
--		[UserId] [nvarchar](128) NULL,
--		[Module_ID] [nvarchar](128) NULL,
--		[CanView] [bit] NULL,
--		[CanEdit] [bit] NULL,
--		[CanDelete] [bit] NULL,
--		[CanAdd] [bit] NULL,
--		[CreatedBy] [nvarchar](max) NULL,
--		[DateCreated] [datetime] NULL,
--		[UpdateBy] [nvarchar](max) NULL,
--		[DateUpdated] [datetime] NULL,
--		[IsActive] [bit] NULL,
--	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
--	(
--		[id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserAccounts'))
BEGIN
    --The following statement was imported into the database project as a schema object and named dbo.AspNetUserAccounts.
--CREATE TABLE [dbo].[AspNetUserAccounts](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Account_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserAccounts] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserBaseUsers'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserBaseUsers.
--CREATE TABLE [dbo].[AspNetUserBaseUsers](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[UserBase_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserBaseUsers] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserTeams'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserTeams.
--CREATE TABLE [dbo].[AspNetUserTeams](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Team_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserTeams] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]


END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetModules'))
BEGIN
--The following statement was imported into the database project as a schema object and named dbo.AspNetModules.
--CREATE TABLE [dbo].[AspNetModules](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[Module_ID] [nvarchar](max) NULL,
--	[Module_Name] [nvarchar](max) NULL,
--	[CreatedBy] [nvarchar](max) NULL,
--	[DateCreated] [datetime] NULL,
--	[UpdateBy] [nvarchar](max) NULL,
--	[DateUpdated] [datetime] NULL,
--	[IsActive] [bit] NULL
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserPermission'))
BEGIN
	--The following statement was imported into the database project as a schema object and named dbo.AspNetUserPermission.
--CREATE TABLE [dbo].[AspNetUserPermission](
--		[id] [int] IDENTITY(1,1) NOT NULL,
--		[UserId] [nvarchar](128) NULL,
--		[Module_ID] [nvarchar](128) NULL,
--		[CanView] [bit] NULL,
--		[CanEdit] [bit] NULL,
--		[CanDelete] [bit] NULL,
--		[CanAdd] [bit] NULL,
--		[CreatedBy] [nvarchar](max) NULL,
--		[DateCreated] [datetime] NULL,
--		[UpdateBy] [nvarchar](max) NULL,
--		[DateUpdated] [datetime] NULL,
--		[IsActive] [bit] NULL,
--	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
--	(
--		[id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserAccounts'))
BEGIN
    --The following statement was imported into the database project as a schema object and named dbo.AspNetUserAccounts.
--CREATE TABLE [dbo].[AspNetUserAccounts](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Account_ID] int  NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserAccounts] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserBaseUsers'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserBaseUsers.
--CREATE TABLE [dbo].[AspNetUserBaseUsers](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[UserBase_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserBaseUsers] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserTeams'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserTeams.
--CREATE TABLE [dbo].[AspNetUserTeams](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Team_ID] int  NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserTeams] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]


END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetModules'))
BEGIN
--The following statement was imported into the database project as a schema object and named dbo.AspNetModules.
--CREATE TABLE [dbo].[AspNetModules](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[Module_ID] [nvarchar](max) NULL,
--	[Module_Name] [nvarchar](max) NULL,
--	[CreatedBy] [nvarchar](max) NULL,
--	[DateCreated] [datetime] NULL,
--	[UpdateBy] [nvarchar](max) NULL,
--	[DateUpdated] [datetime] NULL,
--	[IsActive] [bit] NULL
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserPermission'))
BEGIN
	--The following statement was imported into the database project as a schema object and named dbo.AspNetUserPermission.
--CREATE TABLE [dbo].[AspNetUserPermission](
--		[id] [int] IDENTITY(1,1) NOT NULL,
--		[UserId] [nvarchar](128) NULL,
--		[Module_ID] [nvarchar](128) NULL,
--		[CanView] [bit] NULL,
--		[CanEdit] [bit] NULL,
--		[CanDelete] [bit] NULL,
--		[CanAdd] [bit] NULL,
--		[CreatedBy] [nvarchar](max) NULL,
--		[DateCreated] [datetime] NULL,
--		[UpdateBy] [nvarchar](max) NULL,
--		[DateUpdated] [datetime] NULL,
--		[IsActive] [bit] NULL,
--	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
--	(
--		[id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserAccounts'))
BEGIN
    --The following statement was imported into the database project as a schema object and named dbo.AspNetUserAccounts.
--CREATE TABLE [dbo].[AspNetUserAccounts](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Account_ID] int  NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserAccounts] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserBaseUsers'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserBaseUsers.
--CREATE TABLE [dbo].[AspNetUserBaseUsers](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[UserBase_ID] [nvarchar](128) NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserBaseUsers] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetUserTeams'))
BEGIN

--The following statement was imported into the database project as a schema object and named dbo.AspNetUserTeams.
--CREATE TABLE [dbo].[AspNetUserTeams](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](128) NULL,
--	[Team_ID] int  NULL,
--	[DateUpdated] [datetime] NULL,
--	[CreatedBy] [nvarchar](128) NULL,
-- CONSTRAINT [PK_AspNetUserTeams] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]


END
GO

IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AspNetModules'))
BEGIN
--The following statement was imported into the database project as a schema object and named dbo.AspNetModules.
--CREATE TABLE [dbo].[AspNetModules](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[Module_ID] [nvarchar](max) NULL,
--	[Module_Name] [nvarchar](max) NULL,
--	[CreatedBy] [nvarchar](max) NULL,
--	[DateCreated] [datetime] NULL,
--	[UpdateBy] [nvarchar](max) NULL,
--	[DateUpdated] [datetime] NULL,
--	[IsActive] [bit] NULL
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

--ADMIN
IF NOT EXISTS (SELECT 1 FROM AspNetModules WHERE Module_ID IN ( 'm-dashboard', 'm-accounts', 'm-teams', 'm-users', 'm-timekeeps', 'm-support'))
BEGIN
	INSERT INTO AspNetModules (Module_ID, Module_Name, DateCreated, DateUpdated, CreatedBy, UpdateBy, IsActive)
	SELECT  'm-dashboard', 'Dashboard', GETDATE(), GETDATE(), 'nere', 'nere', 1  
	UNION 
	SELECT 'm-accounts','Accounts', GETDATE(), GETDATE(), 'nere', 'nere', 1
	UNION
	SELECT 'm-teams', 'Teams', GETDATE(), GETDATE(), 'nere', 'nere', 1
	UNION 
	SELECT 'm-users', 'Users', GETDATE(), GETDATE(), 'nere', 'nere', 1
	UNION
	SELECT 'm-timekeeps','Timekeeping', GETDATE(), GETDATE(), 'nere', 'nere', 1
	UNION
	SELECT 'm-support', 'Support', GETDATE(), GETDATE(), 'nere', 'nere', 1
END
GO

INSERT INTO AspNetUserPermission
SELECT  au.UserId, am.Module_ID, 1, 1, 1, 1, 'nere', GETDATE(), 'nere', GETDATE(), 1  
FROM UserDetails au, AspNetModules am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.Module_ID = am.Module_ID)
AND au.Role IN ('SA','AM','LA')
GO

INSERT INTO AspNetUserAccounts
SELECT 
au.UserId, 
am.Id, 
GETDATE(), 'nere' 
FROM UserDetails au, Accounts am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserAccounts _ab WHERE _ab.UserId = au.UserId AND _ab.Account_ID = am.Id)
AND au.Role = 'SA'
GO

INSERT INTO AspNetUserTeams
SELECT au.UserId, am.Id, GETDATE(), 'nere' 
FROM UserDetails au, Teams am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserTeams _ab WHERE _ab.UserId = au.UserId AND _ab.Team_ID = am.Id)
AND au.Role = 'SA'
GO

INSERT INTO AspNetUserBaseUsers
SELECT au.UserId, am.Id,  GETDATE(), 'nere' 
FROM UserDetails au, AspNetUsers am
WHERE 
NOT EXISTS (SELECT 1 FROM AspNetUserBaseUsers _ab WHERE _ab.UserId = au.UserId AND _ab.UserBase_ID = am.Id)
AND 
au.Role = 'SA'

--SELECT * FROM AspNetUserBaseUsers WHERE UserId = 'ae1fc92e-de61-4ca9-87b1-bd1a8305b028'
--SELECT * FROM UserDetails
--SELECT * FROM AspNetUsers
GO

INSERT INTO AspNetUserAccounts
SELECT 
am.UserId, 
am.AccountId, 
GETDATE(), 'nere' 
FROM AccountManagers am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserAccounts _ab WHERE _ab.UserId = am.UserId AND _ab.Account_ID = am.AccountId)
GO

INSERT INTO AspNetUserTeams
SELECT am.UserId, tm.Id, GETDATE(), 'nere' 
FROM AccountManagers am
JOIN Teams tm ON tm.AccountId = am.AccountId
JOIN UserDetails au ON au.UserId = am.UserId
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserTeams _ab WHERE _ab.UserId = au.UserId AND _ab.Team_ID = tm.Id)
GO

INSERT INTO AspNetUserBaseUsers
SELECT am.UserId, ta.UserId, GETDATE(), 'nere' 
FROM AccountManagers am
JOIN Teams tm ON tm.AccountId = am.AccountId
JOIN TeamAgents ta on ta.TeamId = tm.Id
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserBaseUsers _ab WHERE _ab.UserId = am.UserId AND _ab.UserBase_ID = ta.UserId)
GO

INSERT INTO AspNetUserPermission
SELECT  au.UserId, am.Module_ID, 1, 1, 1, 1, 'nere', GETDATE(), 'nere', GETDATE(), 1  
FROM UserDetails au, AspNetModules am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.Module_ID = am.Module_ID)
AND au.Role = 'AG'
GO

INSERT INTO AspNetUserAccounts
SELECT 
am.UserId, 
t.AccountId, 
GETDATE(), 'nere' 
FROM TeamAgents am
JOIN Teams t ON t.Id = am.TeamId 
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserAccounts _ab WHERE _ab.UserId = am.UserId AND _ab.Account_ID = t.AccountId)
GO

INSERT INTO AspNetUserTeams
SELECT am.UserId, t.Id, GETDATE(), 'nere' 
FROM TeamAgents am
JOIN Teams t ON t.Id = am.TeamId 
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserTeams _ab WHERE _ab.UserId = am.UserId AND _ab.Team_ID = t.Id)
GO

INSERT INTO AspNetUserBaseUsers
SELECT am.UserId, ta.UserId, GETDATE(), 'nere' 
FROM TeamAgents am
JOIN Teams t ON t.Id = am.TeamId 
JOIN TeamAgents ta ON ta.TeamId = am.TeamId
JOIN UserDetails au ON au.UserId = am.UserId
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserBaseUsers _ab WHERE _ab.UserId = au.UserId AND _ab.UserBase_ID = ta.UserId)
GO

INSERT INTO AspNetUserAccounts
SELECT 
am.UserId, 
t.AccountId, 
GETDATE(), 'nere' 
FROM TeamSupervisors am
JOIN Teams t ON t.Id = am.TeamId 
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserAccounts _ab WHERE _ab.UserId = am.UserId AND _ab.Account_ID = t.AccountId)
GO

INSERT INTO AspNetUserTeams
SELECT am.UserId, am.TeamId, GETDATE(), 'nere' 
FROM TeamSupervisors am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserTeams _ab WHERE _ab.UserId = am.UserId AND _ab.Team_ID = am.TeamId)
GO

INSERT INTO AspNetUserBaseUsers
SELECT am.UserId, ta.UserId, GETDATE(), 'nere' 
FROM TeamSupervisors am
JOIN Teams t ON t.Id = am.TeamId 
JOIN TeamAgents ta ON ta.TeamId = am.TeamId
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserBaseUsers _ab WHERE _ab.UserId = am.UserId AND _ab.UserBase_ID = ta.UserId)
GO


--SELECT * FROM AspNetUserAccounts WHERE UserId = '67183b42-9b6e-40db-a6cf-a59f48c25dc1'

GO


--SELECT * FROM TeamSupervisors 


GO
