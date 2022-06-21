if exists (select * from sys.objects where name='sp_SetTryMobilePhoneAuthorization' and type='P')
	drop procedure sp_SetTryMobilePhoneAuthorization
GO

create procedure sp_SetTryMobilePhoneAuthorization(@MOBILE_PHONE	varchar(20),
												   @SMS_COUNT		int)
AS
	update MOBILE_PHONE_AUTHORIZATION
	set    SMS_COUNT = @SMS_COUNT
	where  MOBILE_PHONE = @MOBILE_PHONE
GO
