if exists (select * from sys.objects where name='sp_GetBankBranches' and type='P')
	drop procedure dbo.sp_GetBankBranches
GO

create procedure dbo.sp_GetBankBranches(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.BANK_BRANCH
GO
