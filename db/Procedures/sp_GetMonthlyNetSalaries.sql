if exists (select * from sys.objects where name='sp_GetMonthlyNetSalaries' and type='P')
	drop procedure dbo.sp_GetMonthlyNetSalaries
GO

create procedure dbo.sp_GetMonthlyNetSalaries (@LANGUAGE_CODE as char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.MONTHLY_NET_SALARY
GO
