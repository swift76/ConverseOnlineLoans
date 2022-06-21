if exists (select * from sys.objects where name='sp_SaveClientDataFromBank' and type='P')
	drop procedure dbo.sp_SaveClientDataFromBank
GO

create procedure dbo.sp_SaveClientDataFromBank(@APPLICATION_ID 				uniqueidentifier,
											   @FIRST_NAME_EN				varchar(20),
											   @LAST_NAME_EN				varchar(20),
											   @MOBILE_PHONE_2				varchar(20) = null,
											   @EMAIL						varchar(70) = null,
											   @BIRTH_PLACE_CODE			char(2),
											   @CITIZENSHIP_CODE			char(2),
											   @REGISTRATION_COUNTRY_CODE	char(2)			= null,
											   @REGISTRATION_STATE_CODE		char(3)			= null,
											   @REGISTRATION_CITY_CODE		char(9)			= null,
											   @REGISTRATION_STREET			nvarchar(150)	= null,
											   @REGISTRATION_BUILDNUM		nvarchar(30)	= null,
											   @REGISTRATION_APARTMENT		nvarchar(30)	= null,
											   @CURRENT_COUNTRY_CODE		char(2)			= null,
											   @CURRENT_STATE_CODE			char(3)			= null,
											   @CURRENT_CITY_CODE			char(9)			= null,
											   @CURRENT_STREET				nvarchar(150)	= null,
											   @CURRENT_BUILDNUM			nvarchar(30)	= null,
											   @CURRENT_APARTMENT			nvarchar(30)	= null)
AS
	update dbo.APPLICATION set
		FIRST_NAME_EN=@FIRST_NAME_EN,
		LAST_NAME_EN=@LAST_NAME_EN,
		MOBILE_PHONE_2=@MOBILE_PHONE_2,
		EMAIL=@EMAIL,
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
	where ID=@APPLICATION_ID
GO
