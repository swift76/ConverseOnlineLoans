if exists (select * from sys.objects where name='sp_GetLoanGettingOptions' and type='P')
	drop procedure dbo.sp_GetLoanGettingOptions
GO

create procedure dbo.sp_GetLoanGettingOptions(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.LOAN_GETTING_OPTION
GO
