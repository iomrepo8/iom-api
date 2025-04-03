CREATE TABLE [dbo].[AspNetUserPermission](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [nvarchar](128) NULL,
		[ModuleCode] [nvarchar](128) NULL,
		[CanView] [bit] NULL,
		[CanEdit] [bit] NULL,
		[CanDelete] [bit] NULL,
		[CanAdd] [bit] NULL,
		[CreatedBy] [nvarchar](150) NULL,
		[DateCreated] [datetime] NULL,
		[UpdateBy] [nvarchar](150) NULL,
		[DateUpdated] [datetime] NULL,
		[IsActive] [bit] NULL,
	 CONSTRAINT [PK_AspNetUserPermission] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]