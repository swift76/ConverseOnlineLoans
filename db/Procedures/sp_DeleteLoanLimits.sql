if exists (select * from sys.objects where name='sp_DeleteLoanLimits' and type='P')
	drop procedure dbo.sp_DeleteLoanLimits
GO

create procedure dbo.sp_DeleteLoanLimits(@LOAN_TYPE_CODE	char(2))
AS
	delete from dbo.LOAN_LIMIT
	where LOAN_TYPE_CODE=@LOAN_TYPE_CODE
GO
