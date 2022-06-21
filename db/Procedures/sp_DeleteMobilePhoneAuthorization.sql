if exists (select * from sys.objects where name='sp_DeleteMobilePhoneAuthorization' and type='P')
	drop procedure sp_DeleteMobilePhoneAuthorization
GO

create procedure sp_DeleteMobilePhoneAuthorization(@MOBILE_PHONE	varchar(20))
AS
	delete from MOBILE_PHONE_AUTHORIZATION where MOBILE_PHONE = @MOBILE_PHONE
GO
