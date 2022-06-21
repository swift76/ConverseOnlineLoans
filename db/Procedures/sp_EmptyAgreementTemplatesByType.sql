if exists (select * from sys.objects where name='sp_EmptyAgreementTemplatesByType' and type='P')
	drop procedure dbo.sp_EmptyAgreementTemplatesByType
GO

create procedure dbo.sp_EmptyAgreementTemplatesByType(@LOAN_TYPE_ID	char(2))
AS
	delete from dbo.AGREEMENT_TEMPLATE_BY_TYPE where LOAN_TYPE_ID=@LOAN_TYPE_ID
GO
