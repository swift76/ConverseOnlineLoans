if exists (select * from sys.objects where name='sp_GetMobilePhoneAuthorization' and type='P')
	drop procedure sp_GetMobilePhoneAuthorization
GO

create procedure sp_GetMobilePhoneAuthorization(@MOBILE_PHONE	varchar(20))
AS
	select SMS_HASH, SMS_SENT_DATE, SMS_COUNT
	from MOBILE_PHONE_AUTHORIZATION with (UPDLOCK)
	where MOBILE_PHONE = @MOBILE_PHONE
GO
