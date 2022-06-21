create or alter procedure sp_GetPartnerCompanyScoring(
	@PARTNER_COMPANY_CODE	varchar(8)
	,@PRODUCT_CATEGORY_CODE	char(2)
)
AS
	select SCORECARD_CODE
	from PARTNER_COMPANY_SCORING
	where PARTNER_COMPANY_CODE=@PARTNER_COMPANY_CODE
		and PRODUCT_CATEGORY_CODE=@PRODUCT_CATEGORY_CODE
GO