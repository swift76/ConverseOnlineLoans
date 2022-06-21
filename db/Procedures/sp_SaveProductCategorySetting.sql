create or alter procedure sp_SaveProductCategorySetting(
	@SCORECARD_CODE		varchar(3)
	,@PRODUCT_CATEGORY	varchar(2)
	,@SCORE				int
)
AS
	insert into PRODUCT_CATEGORY_SETTING(SCORECARD_CODE, PRODUCT_CATEGORY, SCORE)
	values (@SCORECARD_CODE, @PRODUCT_CATEGORY, @SCORE)
GO
