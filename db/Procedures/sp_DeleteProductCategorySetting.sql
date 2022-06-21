create or alter procedure sp_DeleteProductCategorySetting(@SCORECARD_CODE	varchar(3))
AS
	delete from PRODUCT_CATEGORY_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
