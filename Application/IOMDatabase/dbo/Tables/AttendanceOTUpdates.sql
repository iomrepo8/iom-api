CREATE TABLE [dbo].[AttendanceOTUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HistoryDate] [date] NOT NULL,
	[UserDetailsId] [int] NOT NULL,
	[RegularHours] [decimal](18, 2) NULL,
	[OTHours] [decimal](18, 2) NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[LastUpdated] [datetime] NULL,
	[LastUpdatedBy] [int] NULL,
 CONSTRAINT [PK_AttendanceOTUpdates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO