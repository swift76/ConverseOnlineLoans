if exists (select * from sys.objects where name='STATE' and type='U')
	drop table dbo.STATE
GO

CREATE TABLE dbo.STATE (
	CODE 	char(3)			NOT NULL,
	NAME_AM	nvarchar(50)	NOT NULL,
	NAME_EN varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iSTATE1 ON dbo.STATE(CODE)
GO
