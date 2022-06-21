if exists (select * from sys.objects where name='DOCUMENT_TYPE' and type='U')
	drop table dbo.DOCUMENT_TYPE
GO

CREATE TABLE dbo.DOCUMENT_TYPE (
	CODE 	char(1)			NOT NULL,
	NAME_AM	nvarchar(50)	NOT NULL,
	NAME_EN	varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iDOCUMENT_TYPE1 ON dbo.DOCUMENT_TYPE(CODE)
GO
