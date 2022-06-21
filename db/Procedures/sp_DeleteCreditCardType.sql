if exists (select * from sys.objects where name='sp_DeleteCreditCardType' and type='P')
	drop procedure dbo.sp_DeleteCreditCardType
GO

create procedure dbo.sp_DeleteCreditCardType(@LOAN_TYPE_ID	char(2))
AS
	delete from dbo.CREDIT_CARD_TYPE where LOAN_TYPE_ID=@LOAN_TYPE_ID
GO
