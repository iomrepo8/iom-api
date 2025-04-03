CREATE TABLE [dbo].[TeamTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Created] [datetime] NOT NULL DEFAULT (getdate()),
	[IsActive] [bit] NOT NULL DEFAULT ((0)),
	[EnableNotification] [bit] NOT NULL DEFAULT ((0)),
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TeamTasks]  WITH CHECK ADD  CONSTRAINT [TeamTask_fk] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TeamTasks] CHECK CONSTRAINT [TeamTask_fk]
GO
