if exists (select * from sys.objects where name='sp_GetClientDataForCardValidation' and type='P')
	drop procedure dbo.sp_GetClientDataForCardValidation
GO

create procedure dbo.sp_GetClientDataForCardValidation(@APPLICATION_ID	uniqueidentifier)

AS
	select	CLIENT_CODE,
			FIRST_NAME_EN,
			LAST_NAME_EN,
			LOAN_TYPE_ID,
			CURRENCY_CODE
	from dbo.APPLICATION
	where ID = @APPLICATION_ID
GO
