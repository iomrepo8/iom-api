CREATE TABLE [dbo].[Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ContactPerson] [nvarchar](255) NULL,
	[EmailAddress] [nvarchar](255) NULL,
	[OfficeAddress] [nvarchar](255) NULL,
	[Website] [nvarchar](255) NULL,
	[Created] [datetime] NOT NULL DEFAULT (getdate()),
	[IsActive] [bit] NOT NULL DEFAULT ((1)),
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
	[SeatCode] [nvarchar](50) NULL,
	[SeatSlot] [int] NULL,
 CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO


