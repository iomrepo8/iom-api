CREATE TABLE [dbo].[AspNetModules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleCode] [varchar](150) NULL,
	[Name] [varchar](150) NULL,
	[CreatedBy] [varchar](150) NULL,
	[DateCreated] [datetime] NULL,
	[UpdateBy] [varchar](150) NULL,
	[DateUpdated] [datetime] NULL,
	[IsActive] [bit] NULL,
	[ParentModuleCode] [varchar](150) NULL,
	[Order] [int] NULL,
	[SubModuleOrder] [tinyint] NULL,
 CONSTRAINT [PK_AspNetModules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO