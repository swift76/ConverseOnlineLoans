if exists (select * from sys.objects where name='FICO_SETTING' and type='U')
	drop table dbo.FICO_SETTING
GO

CREATE TABLE dbo.FICO_SETTING (
	SCORECARD_CODE	varchar(3)	NOT NULL,
	VALUE_FROM		int			NOT NULL,
	VALUE_TO		int			NOT NULL,
	SCORE			int			NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iFICO_SETTING1 ON dbo.FICO_SETTING(SCORECARD_CODE, VALUE_FROM, VALUE_TO)
GO
