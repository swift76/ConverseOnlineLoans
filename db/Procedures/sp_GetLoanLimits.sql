if exists (select * from sys.objects where name='sp_GetLoanLimits' and type='P')
	drop procedure dbo.sp_GetLoanLimits
GO

create procedure dbo.sp_GetLoanLimits(@LOAN_TYPE_CODE	char(2),
									  @CURRENCY			char(3))
AS
	select FROM_AMOUNT, TO_AMOUNT
	from dbo.LOAN_LIMIT
	where LOAN_TYPE_CODE = @LOAN_TYPE_CODE and CURRENCY=@CURRENCY
GO
