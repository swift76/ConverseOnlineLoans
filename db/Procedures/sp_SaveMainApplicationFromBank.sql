if exists (select * from sys.objects where name='sp_SaveMainApplicationFromBank' and type='P')
	drop procedure dbo.sp_SaveMainApplicationFromBank
GO

create procedure dbo.sp_SaveMainApplicationFromBank(@APPLICATION_ID 			uniqueidentifier,
													@INTEREST					money,
													@FINAL_AMOUNT				money,
													@PERIOD_TYPE_CODE			char(2),
													@REPAY_DAY					tinyint,
													@COMMUNICATION_TYPE_CODE	char(1)			= null,
													@LOAN_GETTING_OPTION_CODE	char(1)			= NULL,
													@ACCOUNT_CODE				varchar(16)		= NULL,
													@IS_NEW_CARD				bit				= NULL,
													@CREDIT_CARD_TYPE_CODE		char(3)			= NULL,
													@IS_CARD_DELIVERY			bit				= NULL,
													@CARD_DELIVERY_ADDRESS		nvarchar(150)	= NULL,
													@BANK_BRANCH_CODE			char(3)			= NULL,
													@MOBILE_PHONE_2				varchar(20)		= NULL,
													@CARD_RECOVERY_CODE			varchar(20)		= NULL,
													@FIRST_NAME_EN				varchar(20)		= null,
													@LAST_NAME_EN				varchar(20)		= null,
													@BIRTH_PLACE_CODE			char(2)			= null,
													@CITIZENSHIP_CODE			char(2)			= null,
													@EMAIL						varchar(70)		= null,
													@REGISTRATION_COUNTRY_CODE	char(2)			= null,
													@REGISTRATION_STATE_CODE	char(3)			= null,
													@REGISTRATION_CITY_CODE		char(9)			= null,
													@REGISTRATION_STREET		nvarchar(150)	= null,
													@REGISTRATION_BUILDNUM		nvarchar(30)	= null,
													@REGISTRATION_APARTMENT		nvarchar(30)	= null,
													@CURRENT_COUNTRY_CODE		char(2)			= null,
													@CURRENT_STATE_CODE			char(3)			= null,
													@CURRENT_CITY_CODE			char(9)			= null,
													@CURRENT_STREET				nvarchar(150)	= null,
													@CURRENT_BUILDNUM			nvarchar(30)	= null,
													@CURRENT_APARTMENT			nvarchar(30)	= null)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @STATUS tinyint
		select @STATUS = STATUS_ID from dbo.APPLICATION with (updlock) where ID = @APPLICATION_ID

		if not @STATUS in (5, 8)
			RAISERROR ('Հայտը տվյալների լրացման կարգավիճակում չէ', 17, 1)

		update dbo.APPLICATION set
			INTEREST=@INTEREST,
			FINAL_AMOUNT=@FINAL_AMOUNT,
			PERIOD_TYPE_CODE=@PERIOD_TYPE_CODE,
			REPAY_DAY=@REPAY_DAY,
			COMMUNICATION_TYPE_CODE=@COMMUNICATION_TYPE_CODE,
			LOAN_GETTING_OPTION_CODE=@LOAN_GETTING_OPTION_CODE,
			IS_NEW_CARD=@IS_NEW_CARD,
			CREDIT_CARD_TYPE_CODE=@CREDIT_CARD_TYPE_CODE,
			IS_CARD_DELIVERY=@IS_CARD_DELIVERY,
			CARD_DELIVERY_ADDRESS=dbo.ahf_ANSI2Unicode(@CARD_DELIVERY_ADDRESS),
			BANK_BRANCH_CODE=@BANK_BRANCH_CODE,
			MOBILE_PHONE_2=@MOBILE_PHONE_2,
			CARD_RECOVERY_CODE=@CARD_RECOVERY_CODE,
			FIRST_NAME_EN=@FIRST_NAME_EN,
			LAST_NAME_EN=@LAST_NAME_EN,
			BIRTH_PLACE_CODE=@BIRTH_PLACE_CODE,
			CITIZENSHIP_CODE=@CITIZENSHIP_CODE,
			REGISTRATION_COUNTRY_CODE=@REGISTRATION_COUNTRY_CODE,
			REGISTRATION_STATE_CODE=@REGISTRATION_STATE_CODE,
			REGISTRATION_CITY_CODE=@REGISTRATION_CITY_CODE,
			REGISTRATION_STREET=dbo.ahf_ANSI2Unicode(@REGISTRATION_STREET),
			REGISTRATION_BUILDNUM=dbo.ahf_ANSI2Unicode(@REGISTRATION_BUILDNUM),
			REGISTRATION_APARTMENT=dbo.ahf_ANSI2Unicode(@REGISTRATION_APARTMENT),
			CURRENT_COUNTRY_CODE=@CURRENT_COUNTRY_CODE,
			CURRENT_STATE_CODE=@CURRENT_STATE_CODE,
			CURRENT_CITY_CODE=@CURRENT_CITY_CODE,
			CURRENT_STREET=dbo.ahf_ANSI2Unicode(@CURRENT_STREET),
			CURRENT_BUILDNUM=dbo.ahf_ANSI2Unicode(@CURRENT_BUILDNUM),
			CURRENT_APARTMENT=dbo.ahf_ANSI2Unicode(@CURRENT_APARTMENT)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
