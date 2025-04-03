CREATE TABLE [dbo].[TaskGroupItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](150) NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_TaskGroupItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TaskGroupItems]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupItems_IOMTasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[IOMTasks] ([Id])
GO

ALTER TABLE [dbo].[TaskGroupItems] CHECK CONSTRAINT [FK_TaskGroupItems_IOMTasks]
GO

ALTER TABLE [dbo].[TaskGroupItems]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupItems_TaskGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[TaskGroups] ([Id])
GO

ALTER TABLE [dbo].[TaskGroupItems] CHECK CONSTRAINT [FK_TaskGroupItems_TaskGroups]
GO
