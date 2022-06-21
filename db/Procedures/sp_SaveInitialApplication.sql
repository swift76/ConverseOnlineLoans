if exists (select * from sys.objects where name='sp_SaveInitialApplication' and type='P')
	drop procedure dbo.sp_SaveInitialApplication
GO

create procedure dbo.sp_SaveInitialApplication(@SOURCE_ID					tinyint,
											   @LOAN_TYPE_ID				char(2),
											   @FIRST_NAME_AM				nvarchar(20)	= null,
											   @LAST_NAME_AM				nvarchar(20)	= null,
											   @PATRONYMIC_NAME_AM			nvarchar(20)	= null,
											   @BIRTH_DATE					date			= null,
											   @SOCIAL_CARD_NUMBER			char(10)        = null,
											   @DOCUMENT_TYPE_CODE			char(1)			= null,
											   @DOCUMENT_NUMBER				char(9)			= null,
											   @DOCUMENT_GIVEN_BY			char(3)			= null,
											   @DOCUMENT_GIVEN_DATE			date			= null,
											   @DOCUMENT_EXPIRY_DATE		date			= null,
											   @INITIAL_AMOUNT				money			= null,
											   @CURRENCY_CODE				char(3)			= null,
											   @ORGANIZATION_ACTIVITY_CODE	char(2)			= null,
											   @CUSTOMER_USER_ID			int				= null,
											   @SHOP_USER_ID				int				= null,
											   @PARTNER_COMPANY_CODE		varchar(8)		= null,
											   @OPERATION_DETAILS			nvarchar(max),
											   @ID							uniqueidentifier	output,
											   @IS_SUBMIT					bit)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		if not @ID is null
			delete from dbo.APPLICATION where ID = @ID

		declare @IS_PREAPPROVED bit,@STATUS_ID tinyint

		select @IS_PREAPPROVED=IS_PREAPPROVED from LOAN_TYPE
		where CODE=@LOAN_TYPE_ID

		if @IS_SUBMIT=0
			set @STATUS_ID=0
		else
		begin
			if @IS_PREAPPROVED=0
				set @STATUS_ID=1
			else
				set @STATUS_ID=3
		end

		set @ID = newid()
		insert into dbo.APPLICATION(
			CREATION_DATE,
			ID,
			SOURCE_ID,
			LOAN_TYPE_ID,
			STATUS_ID,
			INITIAL_AMOUNT,
			CURRENCY_CODE,
			CUSTOMER_USER_ID,
			SHOP_USER_ID,
			PARTNER_COMPANY_CODE,
			TO_BE_SYNCHRONIZED,
			FIRST_NAME_AM,
			LAST_NAME_AM,
			PATRONYMIC_NAME_AM,
			BIRTH_DATE,
			SOCIAL_CARD_NUMBER,
			ORGANIZATION_ACTIVITY_CODE,
			DOCUMENT_TYPE_CODE,
			DOCUMENT_NUMBER,
			DOCUMENT_GIVEN_BY,
			DOCUMENT_GIVEN_DATE,
			DOCUMENT_EXPIRY_DATE)
		values(
			getdate(),
			@ID,
			@SOURCE_ID,
			@LOAN_TYPE_ID,
			@STATUS_ID,
			@INITIAL_AMOUNT,
			@CURRENCY_CODE,
			@CUSTOMER_USER_ID,
			@SHOP_USER_ID,
			@PARTNER_COMPANY_CODE,
			1,
			@FIRST_NAME_AM,
			@LAST_NAME_AM,
			@PATRONYMIC_NAME_AM,
			@BIRTH_DATE,
			@SOCIAL_CARD_NUMBER,
			@ORGANIZATION_ACTIVITY_CODE,
			@DOCUMENT_TYPE_CODE,
			@DOCUMENT_NUMBER,
			@DOCUMENT_GIVEN_BY,
			@DOCUMENT_GIVEN_DATE,
			@DOCUMENT_EXPIRY_DATE)

		if not @CUSTOMER_USER_ID is null
			update dbo.CUSTOMER_USER set
				DOCUMENT_TYPE_CODE	= @DOCUMENT_TYPE_CODE,
				DOCUMENT_NUMBER		= @DOCUMENT_NUMBER
			where APPLICATION_USER_ID = @CUSTOMER_USER_ID

		insert into dbo.APPLICATION_OPERATION_LOG
			(CUSTOMER_USER_ID, SHOP_USER_ID, PARTNER_COMPANY_CODE, LOAN_TYPE_ID, OPERATION_CODE, APPLICATION_ID, OPERATION_DETAILS)
		values
			(@CUSTOMER_USER_ID, @SHOP_USER_ID, @PARTNER_COMPANY_CODE, @LOAN_TYPE_ID, 'CREATE', @ID, @OPERATION_DETAILS)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
