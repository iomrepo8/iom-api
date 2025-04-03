CREATE TABLE [dbo].[Teams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[ShiftSchedule] [nvarchar](255) NULL,
	[LocationId] [int] NULL,
	[ShiftId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK__Teams__3214EC079D1FF261] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Teams] ADD  CONSTRAINT [DF__Teams__IsActive__55009F39]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Teams] ADD  CONSTRAINT [DF__Teams__Created__540C7B00]  DEFAULT (getdate()) FOR [Created]
GO

ALTER TABLE [dbo].[Teams]  WITH CHECK ADD  CONSTRAINT [FK_Teams_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([Id])
GO

ALTER TABLE [dbo].[Teams] CHECK CONSTRAINT [FK_Teams_Location]
GO

ALTER TABLE [dbo].[Teams]  WITH CHECK ADD  CONSTRAINT [Teams_fk] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Teams] CHECK CONSTRAINT [Teams_fk]
GO