create or alter procedure sp_GetProductCategorySetting(
	@SCORECARD_CODE		varchar(3)
	,@PRODUCT_CATEGORY	varchar(2)
)
AS
	select SCORE
	from PRODUCT_CATEGORY_SETTING
	where SCORECARD_CODE = @SCORECARD_CODE
		and PRODUCT_CATEGORY = @PRODUCT_CATEGORY
GO
