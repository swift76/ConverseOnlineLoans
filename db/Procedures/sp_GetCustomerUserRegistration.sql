if exists (select * from sys.objects where name='sp_GetCustomerUserRegistration' and type='P')
	drop procedure dbo.sp_GetCustomerUserRegistration
GO

create procedure dbo.sp_GetCustomerUserRegistration(@PROCESS_ID	uniqueidentifier)
AS
	select	PROCESS_ID,
			VERIFICATION_CODE,
			FIRST_NAME,
			LAST_NAME,
			MOBILE_PHONE,
			EMAIL,
			SOCIAL_CARD_NUMBER,
			HASH
	from dbo.CUSTOMER_USER_REGISTRATION
	where PROCESS_ID = @PROCESS_ID
GO
