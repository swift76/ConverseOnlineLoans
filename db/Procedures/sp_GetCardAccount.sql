if exists (select * from sys.objects where name='sp_GetCardAccount' and type='P')
	drop procedure dbo.sp_GetCardAccount
GO

create procedure dbo.sp_GetCardAccount(@CODE char(2))

AS
	select IS_CARD_ACCOUNT
	from dbo.LOAN_TYPE
	where CODE = @CODE
GO
