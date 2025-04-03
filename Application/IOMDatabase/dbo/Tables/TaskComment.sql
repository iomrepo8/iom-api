CREATE TABLE [dbo].[TaskComment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskHistoryId] [int] NOT NULL,
	[Comment] [varchar](65) NOT NULL,
	[ChildOf] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_TaskComment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO