CREATE TABLE [dbo].[UserDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](125) NOT NULL,
	[LastName] [nvarchar](125) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[EmployeeStatus] [nvarchar](255) NULL,
	[HourlyRate] [decimal](8, 2) NULL,
	[Role] [nvarchar](255) NULL,
	[Shift] [nvarchar](255) NULL,
	[WeekSchedule] [nvarchar](255) NULL,
	[Image] [nvarchar](max) NULL,
	[TemporaryPassword] [bit] NOT NULL,
	[LocationId] [int] NULL,
	[Created] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[IsLocked] [bit] NULL,
	[EmployeeStatusId] [int] NULL,
	[EmployeeShiftId] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[AccountId] [int] NULL,
	[StaffId] [NVARCHAR](20) NULL,
    [IsUnrestrictedIp] BIT NOT NULL DEFAULT 1, 
    [TimeZoneId] INT NULL , 
    [IPAddress] VARCHAR(50) NULL, 
    CONSTRAINT [PK__UserDeta__3214EC07DF005F4E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF) ON [PRIMARY], 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__First__5E8A0973]  DEFAULT ('') FOR [FirstName]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__LastN__5F7E2DAC]  DEFAULT ('') FOR [LastName]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetail__Name__607251E5]  DEFAULT ('') FOR [Name]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__Hourl__59C55456]  DEFAULT ((0.0)) FOR [HourlyRate]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__Tempo__5AB9788F]  DEFAULT ((1)) FOR [TemporaryPassword]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__Creat__5BAD9CC8]  DEFAULT (getdate()) FOR [Created]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__IsLoc__5CA1C101]  DEFAULT ((0)) FOR [IsLocked]
GO

ALTER TABLE [dbo].[UserDetails] ADD  CONSTRAINT [DF__UserDetai__IsDel__5D95E53A]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserDetails_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([Id])
GO

ALTER TABLE [dbo].[UserDetails] CHECK CONSTRAINT [FK_UserDetails_Location]
GO

ALTER TABLE [dbo].[UserDetails]  WITH CHECK ADD  CONSTRAINT [UserDetailModels_fk] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserDetails] CHECK CONSTRAINT [UserDetailModels_fk]
GO
