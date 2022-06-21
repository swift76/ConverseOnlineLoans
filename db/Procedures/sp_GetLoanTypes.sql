if exists (select * from sys.objects where name='sp_GetLoanTypes' and type='P')
	drop procedure dbo.sp_GetLoanTypes
GO

create procedure dbo.sp_GetLoanTypes(@LANGUAGE_CODE	char(2))
AS
	declare @CurrentDate date=getdate()
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME,
		IS_OVERDRAFT,
		IS_CARD_ACCOUNT
	from dbo.LOAN_TYPE
	where @CurrentDate between isnull(FROM_DATE,@CurrentDate) and isnull(TO_DATE,@CurrentDate)
GO
