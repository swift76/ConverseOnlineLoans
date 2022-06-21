if exists (select * from sys.objects where name='sp_SetTryCreditCardAuthorization' and type='P')
	drop procedure dbo.sp_SetTryCreditCardAuthorization
GO

create procedure dbo.sp_SetTryCreditCardAuthorization(@APPLICATION_ID uniqueidentifier,
													 @TRY_COUNT		 int)
AS
	update dbo.CREDIT_CARD_AUTHORIZATION
	set    TRY_COUNT = @TRY_COUNT
	where  APPLICATION_ID = @APPLICATION_ID
GO
