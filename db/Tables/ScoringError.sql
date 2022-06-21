﻿if exists (select * from sys.objects where name='SCORING_ERROR' and type='U')
	drop table dbo.SCORING_ERROR
GO

CREATE TABLE dbo.SCORING_ERROR (
	DATE			datetime			NOT NULL default getdate(),
	APPLICATION_ID	uniqueidentifier	NULL,
	OPERATION		varchar(200)		NOT NULL,
	ERROR_MESSAGE	nvarchar(max)		NOT NULL
)
GO

 CREATE CLUSTERED INDEX iSCORING_ERROR1 ON dbo.SCORING_ERROR(DATE)
GO
