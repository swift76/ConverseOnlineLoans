if exists (select * from sys.objects where name='AGE_SETTING' and type='U')
	drop table dbo.AGE_SETTING
GO

CREATE TABLE dbo.AGE_SETTING (
	AGE		tinyint	NOT NULL,
	SCORE	int		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iAGE_SETTING1 ON dbo.AGE_SETTING(AGE)
GO
