if exists (select * from sys.objects where name='sp_GetLoanOverdraft' and type='P')
	drop procedure dbo.sp_GetLoanOverdraft
GO

create procedure dbo.sp_GetLoanOverdraft(@CODE char(2))

AS
	select IS_OVERDRAFT
	from dbo.LOAN_TYPE
	where CODE = @CODE
GO
