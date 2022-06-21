if exists (select * from sys.objects where name='sp_StartCustomerUserRegistration' and type='P')
	drop procedure dbo.sp_StartCustomerUserRegistration
GO

create procedure dbo.sp_StartCustomerUserRegistration(
	@PROCESS_ID				uniqueidentifier,
	@VERIFICATION_CODE		char(4),
	@FIRST_NAME				nvarchar(20),
	@LAST_NAME				nvarchar(20),
	@SOCIAL_CARD_NUMBER		char(10),
	@MOBILE_PHONE			varchar(20),
	@EMAIL					varchar(70) = null,
	@HASH					varchar(1000)
)

AS
	insert into dbo.CUSTOMER_USER_REGISTRATION
		(PROCESS_ID, VERIFICATION_CODE, FIRST_NAME, LAST_NAME, MOBILE_PHONE, EMAIL, SOCIAL_CARD_NUMBER, HASH)
	values
		(@PROCESS_ID, @VERIFICATION_CODE, @FIRST_NAME, @LAST_NAME, @MOBILE_PHONE, @EMAIL, @SOCIAL_CARD_NUMBER, @HASH)
GO
