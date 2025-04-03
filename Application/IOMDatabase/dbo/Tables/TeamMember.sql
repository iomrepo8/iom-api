CREATE TABLE [dbo].[TeamMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserDetailsId] [int] NOT NULL,
	[TeamId] [int] NOT NULL,
	[CreatedDateUtc] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[UpdatedDateUtc] [datetime] NULL,
 CONSTRAINT [PK_TeamMember] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TeamMember] ADD  CONSTRAINT [DF_Table_1_Created]  DEFAULT (getutcdate()) FOR [CreatedDateUtc]
GO

ALTER TABLE [dbo].[TeamMember] ADD  CONSTRAINT [DF_TeamMember_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[TeamMember]  WITH CHECK ADD  CONSTRAINT [FK_TeamMember_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([Id])
GO

ALTER TABLE [dbo].[TeamMember] CHECK CONSTRAINT [FK_TeamMember_Teams]
GO

ALTER TABLE [dbo].[TeamMember]  WITH CHECK ADD  CONSTRAINT [FK_TeamMember_UserDetails] FOREIGN KEY([UserDetailsId])
REFERENCES [dbo].[UserDetails] ([Id])
GO

ALTER TABLE [dbo].[TeamMember] CHECK CONSTRAINT [FK_TeamMember_UserDetails]
GO
