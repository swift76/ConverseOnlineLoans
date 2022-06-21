if exists (select * from sys.objects where name='sp_SaveAgreementTemplateByType' and type='P')
	drop procedure dbo.sp_SaveAgreementTemplateByType
GO

create procedure dbo.sp_SaveAgreementTemplateByType(@LOAN_TYPE_ID	char(2),
													@CURRENCY_CODE	char(3),
													@TEMPLATE_CODE	char(4),
													@TEMPLATE_NAME	nvarchar(50),
													@TERM_FROM		int,
													@TERM_TO		int)
AS
	insert into dbo.AGREEMENT_TEMPLATE_BY_TYPE
		(LOAN_TYPE_ID, CURRENCY_CODE, TEMPLATE_CODE, TEMPLATE_NAME, TERM_FROM, TERM_TO)
	values
		(@LOAN_TYPE_ID, @CURRENCY_CODE, @TEMPLATE_CODE, @TEMPLATE_NAME, @TERM_FROM, @TERM_TO)
GO
