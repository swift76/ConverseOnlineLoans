if exists (select * from sys.objects where name='sp_CreateCustomerUser' and type='P')
	drop procedure dbo.sp_CreateCustomerUser
GO

create procedure dbo.sp_CreateCustomerUser (@FIRST_NAME				nvarchar(50),
											@LAST_NAME				nvarchar(50),
											@SOCIAL_CARD_NUMBER		char(10),
											@MOBILE_PHONE			char(11),
											@EMAIL					varchar(50) = null,
											@HASH					varchar(1000),
											@OPERATION_DETAILS		nvarchar(max),
											@APPLICATION_USER_ID	int OUTPUT)

AS
	BEGIN TRANSACTION

	BEGIN TRY
		insert into dbo.APPLICATION_USER
			(LOGIN, HASH, FIRST_NAME, LAST_NAME, EMAIL, USER_STATE_ID, USER_ROLE_ID)
		values
			(@SOCIAL_CARD_NUMBER, @HASH, @FIRST_NAME, @LAST_NAME, @EMAIL, 1 /* Ակտիվ */, 3 /* Հաճախորդ */)

		set @APPLICATION_USER_ID = SCOPE_IDENTITY()


		insert into dbo.APPLICATION_USER_OPERATION_LOG
			(APPLICATION_USER_ID, BANK_OBJECT_CODE, BANK_OPERATION_CODE, ENTITY_ID,	OPERATION_DETAILS)
		values
			(@APPLICATION_USER_ID, 'CUSTOMERUSER', 'CREATE', @APPLICATION_USER_ID, @OPERATION_DETAILS)

		insert into dbo.CUSTOMER_USER
			(APPLICATION_USER_ID, FIRST_NAME_AM, LAST_NAME_AM, MOBILE_PHONE, EMAIL, SOCIAL_CARD_NUMBER)
		values
			(@APPLICATION_USER_ID, upper(@FIRST_NAME), upper(@LAST_NAME), @MOBILE_PHONE, @EMAIL, @SOCIAL_CARD_NUMBER)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
