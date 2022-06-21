if exists (select * from sys.objects where name='sp_DeleteCreditCardAuthorization' and type='P')
	drop procedure dbo.sp_DeleteCreditCardAuthorization
GO

create procedure dbo.sp_DeleteCreditCardAuthorization(@APPLICATION_ID	uniqueidentifier)
AS
	delete from dbo.CREDIT_CARD_AUTHORIZATION where APPLICATION_ID = @APPLICATION_ID
GO
