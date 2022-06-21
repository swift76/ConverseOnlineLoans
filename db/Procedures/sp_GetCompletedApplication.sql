if exists (select * from sys.objects where name='sp_GetCompletedApplication' and type='P')
	drop procedure dbo.sp_GetCompletedApplication
GO

create procedure dbo.sp_GetCompletedApplication (@ID uniqueidentifier)
AS
	select  LOAN_TYPE_ID,
			FINAL_AMOUNT,
			PERIOD_TYPE_CODE,
			INTEREST,
			EFFECTIVE_INTEREST,
			MONTHLY_PAID_AMOUNT,
			FIRST_PAID_INTEREST,
			FIRST_PAID_PRINCIPAL_AMOUNT,
			TOTAL_PAID_AMOUNT,
			TOTAL_PAID_INTEREST
	from dbo.APPLICATION
	where ID = @ID
GO
