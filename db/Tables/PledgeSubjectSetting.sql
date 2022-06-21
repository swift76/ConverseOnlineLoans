if exists (select * from sys.objects where name='PLEDGE_SUBJECT_SETTING' and type='U')
	drop table dbo.PLEDGE_SUBJECT_SETTING
GO

CREATE TABLE dbo.PLEDGE_SUBJECT_SETTING (
	ID				int				identity(1, 1),
	PLEDGE_SUBJECT	nvarchar(1000)	NOT NULL,
	SCORE			int				NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iPLEDGE_SUBJECT_SETTING1 ON dbo.PLEDGE_SUBJECT_SETTING(ID)
GO