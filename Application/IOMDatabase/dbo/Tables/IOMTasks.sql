CREATE TABLE [dbo].[IOMTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskNumber] [varchar](50) NULL,
	[Name] [varchar](150) NOT NULL,
	[Description] [varchar](250) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Tasks_IsDeleted]  DEFAULT ((0)),
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Tasks_IsActive]  DEFAULT ((0)),
 [ClickUpRef] NVARCHAR(MAX) NULL, 
    [Trigger] NVARCHAR(MAX) NULL, 
    [Manual] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

