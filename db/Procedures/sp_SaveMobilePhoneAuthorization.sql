if exists (select * from sys.objects where name='sp_SaveMobilePhoneAuthorization' and type='P')
	drop procedure sp_SaveMobilePhoneAuthorization
GO

create procedure sp_SaveMobilePhoneAuthorization(@MOBILE_PHONE	varchar(20),
												 @SMS_HASH 		varchar(1000))
AS
	declare @SMS_COUNT int
	select @SMS_COUNT = SMS_COUNT
	from MOBILE_PHONE_AUTHORIZATION
	where MOBILE_PHONE = @MOBILE_PHONE

	execute sp_DeleteMobilePhoneAuthorization @MOBILE_PHONE

	insert into MOBILE_PHONE_AUTHORIZATION (MOBILE_PHONE, SMS_HASH)
		values (@MOBILE_PHONE, @SMS_HASH)

	if not @SMS_COUNT is null
		update MOBILE_PHONE_AUTHORIZATION
		set SMS_COUNT = @SMS_COUNT + 1
		where MOBILE_PHONE = @MOBILE_PHONE
GO
