if exists (select * from sys.objects where name='sp_GetAgreedApplication' and type='P')
	drop procedure dbo.sp_GetAgreedApplication
GO

create procedure dbo.sp_GetAgreedApplication(@ID	uniqueidentifier)
AS
	select  LOAN_GETTING_OPTION_CODE,
			COMMUNICATION_TYPE_CODE,
			EXISTING_CARD_CODE,
			IS_NEW_CARD,
			CREDIT_CARD_TYPE_CODE,
			IS_CARD_DELIVERY,
			CARD_DELIVERY_ADDRESS,
			BANK_BRANCH_CODE,
			MOBILE_PHONE_2,
			CARD_RECOVERY_CODE,
			IS_ARBITRAGE_CHECKED,
			STATUS_ID,
			LOAN_TYPE_ID,
			convert(bit,case LOAN_TYPE_ID
				when 13 then 0
				else 1
			end) as AGREED_WITH_TERMS
	from dbo.APPLICATION
	where ID = @ID
GO
