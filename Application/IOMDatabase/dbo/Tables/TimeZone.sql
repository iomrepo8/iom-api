﻿CREATE TABLE [dbo].[TimeZone]
(
    [Id] INT IDENTITY(1, 1)  NOT NULL, 
    [Zone] NVARCHAR(100) NULL, 
    [Value] NVARCHAR(100) NULL
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([Id] ASC)
)
