if exists (select * from sys.objects where name='PARTNER_COMPANY' and type='U')
	drop table dbo.PARTNER_COMPANY
GO

CREATE TABLE dbo.PARTNER_COMPANY(
	CODE		varchar(8)		NOT NULL,
	NAME		nvarchar(40)	NULL,
	PARENT_CODE	varchar(8)		NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iPARTNER_COMPANY1 ON dbo.PARTNER_COMPANY(CODE)
GO
