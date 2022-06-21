create or alter procedure sp_SaveProductCategorySetting(
	@PRODUCT_CATEGORY	varchar(2)
	,@SCORE				int
)
AS
	insert into PRODUCT_CATEGORY_SETTING(PRODUCT_CATEGORY, SCORE)
	values (@PRODUCT_CATEGORY, @SCORE)
GO
