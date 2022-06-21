if exists (select * from sys.objects where name='sp_GetGeneralLoanSettings' and type='P')
	drop procedure dbo.sp_GetGeneralLoanSettings
GO

create procedure dbo.sp_GetGeneralLoanSettings
AS
	select
		REPEAT_COUNT,
		REPEAT_DAY_COUNT,
		EXPIRE_DAY_COUNT
	from GENERAL_LOAN_SETTING
GO
