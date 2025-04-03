CREATE TABLE [dbo].[UserTaskGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[GroupId] [int] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_UserTaskGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserTaskGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserTaskGroup_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[UserTaskGroup] CHECK CONSTRAINT [FK_UserTaskGroup_AspNetUsers]
GO

ALTER TABLE [dbo].[UserTaskGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserTaskGroup_TaskGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[TaskGroups] ([Id])
GO

ALTER TABLE [dbo].[UserTaskGroup] CHECK CONSTRAINT [FK_UserTaskGroup_TaskGroups]
GO