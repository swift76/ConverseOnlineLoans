if exists (select * from sys.objects where name='sp_SaveAgreedApplication' and type='P')
	drop procedure dbo.sp_SaveAgreedApplication
GO

create procedure dbo.sp_SaveAgreedApplication(@APPLICATION_ID 			uniqueidentifier,
											  @LOAN_GETTING_OPTION_CODE char(1)		= null,
											  @COMMUNICATION_TYPE_CODE	char(1)		= null,
											  @EXISTING_CARD_CODE		char(16)	= null,
											  @IS_NEW_CARD				bit			= null,
											  @CREDIT_CARD_TYPE_CODE	char(3)		= null,
											  @IS_CARD_DELIVERY			bit			= null,
											  @CARD_DELIVERY_ADDRESS	nvarchar(150) = null,
											  @BANK_BRANCH_CODE			char(3)		= null,
											  @MOBILE_PHONE_2			varchar(20) = null,
											  @CARD_RECOVERY_CODE		varchar(20) = null,
											  @IS_ARBITRAGE_CHECKED		bit			= null,
											  @OPERATION_DETAILS		nvarchar(max),
											  @IS_SUBMIT				bit)

AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @STATUS_ID tinyint, @CUSTOMER_USER_ID int, @PARTNER_COMPANY_CODE varchar(8), @LOAN_TYPE_ID char(2)

		select @STATUS_ID = STATUS_ID, @CUSTOMER_USER_ID = CUSTOMER_USER_ID, @PARTNER_COMPANY_CODE = PARTNER_COMPANY_CODE, @LOAN_TYPE_ID = LOAN_TYPE_ID
		from dbo.APPLICATION with (updlock) where ID = @APPLICATION_ID

		if not @STATUS_ID in (5, 8)
			RAISERROR ('Հայտը պայմանագրի կնքման կարգավիճակում չէ', 17, 1)

		insert into dbo.APPLICATION_OPERATION_LOG
			(CUSTOMER_USER_ID, PARTNER_COMPANY_CODE, LOAN_TYPE_ID, OPERATION_CODE, APPLICATION_ID, OPERATION_DETAILS)
		values
			(@CUSTOMER_USER_ID, @PARTNER_COMPANY_CODE, @LOAN_TYPE_ID, 'EDIT', @APPLICATION_ID, @OPERATION_DETAILS)

		update dbo.APPLICATION set
			LOAN_GETTING_OPTION_CODE= isnull(@LOAN_GETTING_OPTION_CODE, LOAN_GETTING_OPTION_CODE),
			COMMUNICATION_TYPE_CODE	= isnull(@COMMUNICATION_TYPE_CODE,	COMMUNICATION_TYPE_CODE),
			IS_NEW_CARD				= isnull(@IS_NEW_CARD,				IS_NEW_CARD),
			EXISTING_CARD_CODE		= isnull(@EXISTING_CARD_CODE,		EXISTING_CARD_CODE),
			CREDIT_CARD_TYPE_CODE	= isnull(@CREDIT_CARD_TYPE_CODE,	CREDIT_CARD_TYPE_CODE),
			IS_CARD_DELIVERY		= isnull(@IS_CARD_DELIVERY,			IS_CARD_DELIVERY),
			CARD_DELIVERY_ADDRESS	= isnull(@CARD_DELIVERY_ADDRESS,	CARD_DELIVERY_ADDRESS),
			BANK_BRANCH_CODE		= isnull(@BANK_BRANCH_CODE,			BANK_BRANCH_CODE),
			MOBILE_PHONE_2			= isnull(@MOBILE_PHONE_2,			MOBILE_PHONE_2),
			CARD_RECOVERY_CODE		= isnull(@CARD_RECOVERY_CODE,		CARD_RECOVERY_CODE),
			IS_ARBITRAGE_CHECKED	= isnull(@IS_ARBITRAGE_CHECKED,		IS_ARBITRAGE_CHECKED)
		where ID = @APPLICATION_ID

		if @IS_SUBMIT=1
			execute dbo.sp_ChangeApplicationStatus @APPLICATION_ID,15

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
