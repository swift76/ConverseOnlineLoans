if exists (select * from sys.objects where name='BANK_BRANCH' and type='U')
	drop table dbo.BANK_BRANCH
GO

CREATE TABLE dbo.BANK_BRANCH (
	CODE 	char(3)			NOT NULL,
	NAME_AM	nvarchar(50)	NOT NULL,
	NAME_EN	varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iBANK_BRANCH1 ON dbo.BANK_BRANCH(CODE)
GO
