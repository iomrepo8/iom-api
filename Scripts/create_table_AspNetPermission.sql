CREATE TABLE [dbo].[AspNetUserPermission](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NULL,
	[Module_ID] [nvarchar](128) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanDelete] [bit] NULL,
	[CanAdd] [bit] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[DateCreated] [datetime] NULL,
	[UpdateBy] [nvarchar](max) NULL,
	[DateUpdated] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


