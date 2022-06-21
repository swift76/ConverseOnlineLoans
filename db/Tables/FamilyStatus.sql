if exists (select * from sys.objects where name='FAMILY_STATUS' and type='U')
	drop table dbo.FAMILY_STATUS
GO

CREATE TABLE dbo.FAMILY_STATUS (
	CODE 	char(1)			NOT NULL,
	NAME_AM	nvarchar(50)	NOT NULL,
	NAME_EN	varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iFAMILY_STATUS1 ON dbo.FAMILY_STATUS(CODE)
GO
