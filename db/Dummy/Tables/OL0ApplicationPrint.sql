if exists (select * from sys.objects where name='OL0_APPLICATION_PRINT' and type='U')
	drop table dbo.OL0_APPLICATION_PRINT
GO

CREATE TABLE OL0_APPLICATION_PRINT (
	APPLICATION_ID				uniqueidentifier	NOT NULL,
	APPLICATION_PRINT_TYPE_ID	tinyint				NOT NULL,
	CONTENT						varbinary(max)		NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iOL0_APPLICATION_PRINT1 ON dbo.OL0_APPLICATION_PRINT(APPLICATION_ID, APPLICATION_PRINT_TYPE_ID)
GO
