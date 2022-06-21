if exists (select * from sys.objects where name='INCOME_SETTING' and type='U')
	drop table dbo.INCOME_SETTING
GO

CREATE TABLE dbo.INCOME_SETTING (
	SCORECARD_CODE	varchar(3)	NOT NULL,
	VALUE_FROM		money		NOT NULL,
	VALUE_TO		money		NOT NULL,
	SCORE			int			NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iINCOME_SETTING1 ON dbo.INCOME_SETTING(SCORECARD_CODE, VALUE_FROM, VALUE_TO)
GO
