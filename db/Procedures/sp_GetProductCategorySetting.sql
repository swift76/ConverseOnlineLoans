create or alter procedure sp_GetProductCategorySetting(@PRODUCT_CATEGORY	varchar(2))
AS
	select SCORE
	from PRODUCT_CATEGORY_SETTING
	where PRODUCT_CATEGORY = @PRODUCT_CATEGORY
GO
