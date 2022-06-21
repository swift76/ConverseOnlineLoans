if exists (select * from sys.objects where name='sp_RegisterAndSubmitApplication' and type='P')
	drop procedure dbo.sp_RegisterAndSubmitApplication
GO

create procedure dbo.sp_RegisterAndSubmitApplication(
	@FIRST_NAME_AM		nvarchar(20),
	@LAST_NAME_AM		nvarchar(20),
	@PATRONYMIC_NAME_AM	nvarchar(20) = null,
	@BIRTH_DATE			date,
	@SOCIAL_CARD_NUMBER	char(10),
	@DOCUMENT_TYPE_CODE	char(1),
	@DOCUMENT_NUMBER	char(9),
	@MOBILE_PHONE		char(11),
	@EMAIL				varchar(50),
	@OPERATION_DETAILS	nvarchar(max),
	@ID					uniqueidentifier	output
)
AS
	declare @LOAN_TYPE_ID char(2),@CURRENCY_CODE char(3)
	select top 1
		@LOAN_TYPE_ID = LOAN_TYPE_CODE,
		@CURRENCY_CODE = CURRENCY
	from LOAN_LIMIT

	BEGIN TRANSACTION

	BEGIN TRY
		set @ID = newid()
		insert into APPLICATION(
			CREATION_DATE,
			ID,
			SOURCE_ID,
			LOAN_TYPE_ID,
			STATUS_ID,
			INITIAL_AMOUNT,
			CURRENCY_CODE,
			CUSTOMER_USER_ID,
			TO_BE_SYNCHRONIZED,
			FIRST_NAME_AM,
			LAST_NAME_AM,
			PATRONYMIC_NAME_AM,
			BIRTH_DATE,
			SOCIAL_CARD_NUMBER,
			DOCUMENT_TYPE_CODE,
			DOCUMENT_NUMBER,
			MOBILE_PHONE_1,
			EMAIL,
			FIRST_NAME_EN,
			LAST_NAME_EN)
		values(
			getdate(),
			@ID,
			1,
			@LOAN_TYPE_ID,
			1,
			0,
			@CURRENCY_CODE,
			0,
			1,
			@FIRST_NAME_AM,
			@LAST_NAME_AM,
			@PATRONYMIC_NAME_AM,
			@BIRTH_DATE,
			@SOCIAL_CARD_NUMBER,
			@DOCUMENT_TYPE_CODE,
			@DOCUMENT_NUMBER,
			@MOBILE_PHONE,
			@EMAIL,
			@FIRST_NAME_AM,
			@LAST_NAME_AM)

		insert into dbo.APPLICATION_OPERATION_LOG
			(CUSTOMER_USER_ID, LOAN_TYPE_ID, OPERATION_CODE, APPLICATION_ID, OPERATION_DETAILS)
		values
			(0, @LOAN_TYPE_ID, 'CREATE', @ID, @OPERATION_DETAILS)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
