if exists (select * from sys.objects where name='USER_STATE' and type='U')
	drop table dbo.USER_STATE
GO

CREATE TABLE dbo.USER_STATE (
	ID 			tinyint IDENTITY(1,1)	NOT NULL,
	DESCRIPTION	nvarchar(50)			NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iUSER_STATE1 ON dbo.USER_STATE(ID)
GO
