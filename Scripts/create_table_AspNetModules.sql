CREATE TABLE [dbo].[AspNetModules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Module_ID] [nvarchar](max) NULL,
	[Module_Name] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[DateCreated] [datetime] NULL,
	[UpdateBy] [nvarchar](max) NULL,
	[DateUpdated] [datetime] NULL,
	[IsActive] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


