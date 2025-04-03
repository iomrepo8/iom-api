CREATE TABLE [dbo].[UserTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[TaskId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[TeamId] [int] NULL,
	[GroupId] [int] NULL,
 CONSTRAINT [PK_TaskAssignees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_AspNetUsers]
GO

ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_IOMTasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[IOMTasks] ([Id])
GO

ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_IOMTasks]
GO

ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_TaskGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[TaskGroups] ([Id])
GO

ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_TaskGroups]
GO

ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
GO

ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_Teams]
GO
