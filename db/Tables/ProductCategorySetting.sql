if exists (select * from sys.objects where name='PRODUCT_CATEGORY_SETTING' and type='U')
	drop table dbo.PRODUCT_CATEGORY_SETTING
GO

CREATE TABLE dbo.PRODUCT_CATEGORY_SETTING (
	SCORECARD_CODE		varchar(3)	NOT NULL,
	PRODUCT_CATEGORY	varchar(2)	NOT NULL,
	SCORE				int			NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iPRODUCT_CATEGORY_SETTING1 ON dbo.PRODUCT_CATEGORY_SETTING(SCORECARD_CODE, PRODUCT_CATEGORY)
GO
