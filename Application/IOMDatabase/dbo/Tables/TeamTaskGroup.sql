CREATE TABLE [dbo].[TeamTaskGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TeamTaskGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TeamTaskGroup]  WITH CHECK ADD  CONSTRAINT [FK_TeamTaskGroup_TaskGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[TaskGroups] ([Id])
GO

ALTER TABLE [dbo].[TeamTaskGroup] CHECK CONSTRAINT [FK_TeamTaskGroup_TaskGroups]
GO

ALTER TABLE [dbo].[TeamTaskGroup]  WITH CHECK ADD  CONSTRAINT [FK_TeamTaskGroup_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
GO

ALTER TABLE [dbo].[TeamTaskGroup] CHECK CONSTRAINT [FK_TeamTaskGroup_Teams]
GO
