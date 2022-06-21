create or alter procedure sp_DeletePartnerCompanyScoring(@PARTNER_COMPANY_CODE	varchar(8))
AS
	delete from PARTNER_COMPANY_SCORING
	where PARTNER_COMPANY_CODE=@PARTNER_COMPANY_CODE
GO
