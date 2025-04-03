CREATE TABLE [dbo].[Location](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreateDate] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdateDate] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Location] ADD  CONSTRAINT [DF_Location_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
