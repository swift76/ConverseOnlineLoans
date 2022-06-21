if exists (select * from sys.objects where name='CITY' and type='U')
	drop table dbo.CITY
GO

CREATE TABLE dbo.CITY (
	CODE		varchar(30)		NOT NULL,
	STATE_CODE	char(3)			NOT NULL,
	NAME_AM		nvarchar(50)	NOT NULL,
	NAME_EN		varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iCITY1 ON dbo.CITY(CODE)
GO
