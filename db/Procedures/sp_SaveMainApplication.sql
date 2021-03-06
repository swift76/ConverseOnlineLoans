if exists (select * from sys.objects where name='sp_SaveMainApplication' and type='P')
	drop procedure dbo.sp_SaveMainApplication
GO

create procedure dbo.sp_SaveMainApplication(@APPLICATION_ID 			uniqueidentifier,
											@FINAL_AMOUNT				money			= null,
											@PERIOD_TYPE_CODE			char(2)			= null,
											@INTEREST					money			= null,
											@REPAY_DAY					tinyint			= null,
											@GOODS_RECEIVING_CODE		char(1)			= null,
											@GOODS_DELIVERY_ADDRESS		nvarchar(150)	= null,
											@FIRST_NAME_EN				varchar(20)		= null,
											@LAST_NAME_EN				varchar(20)		= null,
											@MOBILE_PHONE_1				varchar(20)		= null,
											@MOBILE_PHONE_2				varchar(20)		= null,
											@FIXED_PHONE				varchar(20)		= null,
											@EMAIL						varchar(70)		= null,
											@BIRTH_PLACE_CODE			char(2)			= null,
											@CITIZENSHIP_CODE			char(2)			= null,
											@REGISTRATION_COUNTRY_CODE	char(2)			= null,
											@REGISTRATION_STATE_CODE	char(3)			= null,
											@REGISTRATION_CITY_CODE		char(9)			= null,
											@REGISTRATION_STREET		nvarchar(150)	= null,
											@REGISTRATION_BUILDNUM		nvarchar(30)	= null,
											@REGISTRATION_APARTMENT		nvarchar(30)	= null,
											@CURRENT_COUNTRY_CODE		char(2)			= null,
											@CURRENT_STATE_CODE			char(3)			= null,
											@CURRENT_CITY_CODE			char(9)			= null,
											@CURRENT_STREET				nvarchar(150)	= null,
											@CURRENT_BUILDNUM			nvarchar(30)	= null,
											@CURRENT_APARTMENT			nvarchar(30)	= null,
											@COMMUNICATION_TYPE_CODE	char(1)			= null,
											@COMPANY_NAME				nvarchar(40)	= null,
											@COMPANY_PHONE				varchar(20)		= null,
											@POSITION					nvarchar(50)	= null,
											@MONTHLY_INCOME_CODE		char(1)			= null,
											@WORKING_EXPERIENCE_CODE	char(1)			= null,
											@FAMILY_STATUS_CODE			char(1)			= null,
											@SHOP_CODE					char(4)			= null,
											@SHOP_USER_ID				int				= null,
											@PRODUCT_CATEGORY_CODE		char(2)			= null,
											@LOAN_TEMPLATE_CODE			char(4)			= null,
											@OVERDRAFT_TEMPLATE_CODE	char(4)			= null,
											@OPERATION_DETAILS			nvarchar(max),
											@IS_SUBMIT					bit)

AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @STATUS_ID tinyint, @CUSTOMER_USER_ID int, @PARTNER_COMPANY_CODE varchar(8), @LOAN_TYPE_ID char(2)

		select @STATUS_ID = STATUS_ID, @CUSTOMER_USER_ID = CUSTOMER_USER_ID, @PARTNER_COMPANY_CODE = PARTNER_COMPANY_CODE, @LOAN_TYPE_ID = LOAN_TYPE_ID
		from dbo.APPLICATION with (updlock) where ID = @APPLICATION_ID

		if not @STATUS_ID in (5, 8)
			RAISERROR ('Հայտը տվյալների լրացման կարգավիճակում չէ', 17, 1)

		insert into dbo.APPLICATION_OPERATION_LOG
			(CUSTOMER_USER_ID, SHOP_USER_ID, PARTNER_COMPANY_CODE, LOAN_TYPE_ID, OPERATION_CODE, APPLICATION_ID, OPERATION_DETAILS)
		values
			(@CUSTOMER_USER_ID, @SHOP_USER_ID, @PARTNER_COMPANY_CODE, @LOAN_TYPE_ID, 'EDIT', @APPLICATION_ID, @OPERATION_DETAILS)

		select @MOBILE_PHONE_1 = MOBILE_PHONE
		from dbo.CUSTOMER_USER where APPLICATION_USER_ID = @CUSTOMER_USER_ID

		update dbo.APPLICATION set
			FINAL_AMOUNT				= isnull(@FINAL_AMOUNT,					FINAL_AMOUNT),
			PERIOD_TYPE_CODE			= isnull(@PERIOD_TYPE_CODE,				PERIOD_TYPE_CODE),
			INTEREST					= isnull(@INTEREST,						INTEREST),
			REPAY_DAY					= isnull(@REPAY_DAY,					REPAY_DAY),
			GOODS_RECEIVING_CODE		= isnull(@GOODS_RECEIVING_CODE,			GOODS_RECEIVING_CODE),
			GOODS_DELIVERY_ADDRESS		= isnull(@GOODS_DELIVERY_ADDRESS,		GOODS_DELIVERY_ADDRESS),
			FIRST_NAME_EN				= isnull(@FIRST_NAME_EN,				FIRST_NAME_EN),
			LAST_NAME_EN				= isnull(@LAST_NAME_EN,					LAST_NAME_EN),
			MOBILE_PHONE_1				= isnull(@MOBILE_PHONE_1,				MOBILE_PHONE_1),
			MOBILE_PHONE_2				= isnull(@MOBILE_PHONE_2,				MOBILE_PHONE_2),
			FIXED_PHONE					= isnull(@FIXED_PHONE,					FIXED_PHONE),
			EMAIL						= isnull(@EMAIL,						EMAIL),
			BIRTH_PLACE_CODE			= isnull(@BIRTH_PLACE_CODE,				BIRTH_PLACE_CODE),
			CITIZENSHIP_CODE			= isnull(@CITIZENSHIP_CODE,				CITIZENSHIP_CODE),
			REGISTRATION_COUNTRY_CODE	= isnull(@REGISTRATION_COUNTRY_CODE,	REGISTRATION_COUNTRY_CODE),
			REGISTRATION_STATE_CODE		= isnull(@REGISTRATION_STATE_CODE,		REGISTRATION_STATE_CODE),
			REGISTRATION_CITY_CODE		= isnull(@REGISTRATION_CITY_CODE,		REGISTRATION_CITY_CODE),
			REGISTRATION_STREET			= isnull(@REGISTRATION_STREET,			REGISTRATION_STREET),
			REGISTRATION_BUILDNUM		= isnull(@REGISTRATION_BUILDNUM,		REGISTRATION_BUILDNUM),
			REGISTRATION_APARTMENT		= isnull(@REGISTRATION_APARTMENT,		REGISTRATION_APARTMENT),
			CURRENT_COUNTRY_CODE		= isnull(@CURRENT_COUNTRY_CODE,			CURRENT_COUNTRY_CODE),
			CURRENT_STATE_CODE			= isnull(@CURRENT_STATE_CODE,			CURRENT_STATE_CODE),
			CURRENT_CITY_CODE			= isnull(@CURRENT_CITY_CODE,			CURRENT_CITY_CODE),
			CURRENT_STREET				= isnull(@CURRENT_STREET,				CURRENT_STREET),
			CURRENT_BUILDNUM			= isnull(@CURRENT_BUILDNUM,				CURRENT_BUILDNUM),
			CURRENT_APARTMENT			= isnull(@CURRENT_APARTMENT,			CURRENT_APARTMENT),
			COMMUNICATION_TYPE_CODE		= isnull(@COMMUNICATION_TYPE_CODE,		COMMUNICATION_TYPE_CODE),
			COMPANY_NAME				= isnull(@COMPANY_NAME,					COMPANY_NAME),
			COMPANY_PHONE				= isnull(@COMPANY_PHONE,				COMPANY_PHONE),
			POSITION					= isnull(@POSITION,						POSITION),
			MONTHLY_INCOME_CODE			= isnull(@MONTHLY_INCOME_CODE,			MONTHLY_INCOME_CODE),
			WORKING_EXPERIENCE_CODE		= isnull(@WORKING_EXPERIENCE_CODE,		WORKING_EXPERIENCE_CODE),
			FAMILY_STATUS_CODE			= isnull(@FAMILY_STATUS_CODE,			FAMILY_STATUS_CODE),
			SHOP_CODE					= isnull(@SHOP_CODE,					SHOP_CODE),
			SHOP_USER_ID				= isnull(@SHOP_USER_ID,					SHOP_USER_ID),
			PRODUCT_CATEGORY_CODE		= isnull(@PRODUCT_CATEGORY_CODE,		PRODUCT_CATEGORY_CODE),
			LOAN_TEMPLATE_CODE			= isnull(@LOAN_TEMPLATE_CODE,			LOAN_TEMPLATE_CODE),
			OVERDRAFT_TEMPLATE_CODE		= isnull(@OVERDRAFT_TEMPLATE_CODE,		OVERDRAFT_TEMPLATE_CODE)
		where ID = @APPLICATION_ID

		if @IS_SUBMIT=1
			execute dbo.sp_ChangeApplicationStatus @APPLICATION_ID, 10

		if not @CUSTOMER_USER_ID is null
			update dbo.CUSTOMER_USER set
				FIRST_NAME_EN				= isnull(@FIRST_NAME_EN,				FIRST_NAME_EN),
				LAST_NAME_EN				= isnull(@LAST_NAME_EN,					LAST_NAME_EN),
				BIRTH_PLACE_CODE 			= isnull(@BIRTH_PLACE_CODE, 			BIRTH_PLACE_CODE),
				CITIZENSHIP_CODE			= isnull(@CITIZENSHIP_CODE,				CITIZENSHIP_CODE),
				EMAIL						= isnull(@EMAIL,						EMAIL),
				FIXED_PHONE					= isnull(@FIXED_PHONE,					FIXED_PHONE),
				MOBILE_PHONE_2				= isnull(@MOBILE_PHONE_2,				MOBILE_PHONE_2),

				REGISTRATION_COUNTRY_CODE	= isnull(@REGISTRATION_COUNTRY_CODE,	REGISTRATION_COUNTRY_CODE),
				REGISTRATION_STATE_CODE		= isnull(@REGISTRATION_STATE_CODE,		REGISTRATION_STATE_CODE),
				REGISTRATION_CITY_CODE		= isnull(@REGISTRATION_CITY_CODE,		REGISTRATION_CITY_CODE),
				REGISTRATION_STREET			= isnull(@REGISTRATION_STREET,			REGISTRATION_STREET),
				REGISTRATION_BUILDNUM		= isnull(@REGISTRATION_BUILDNUM,		REGISTRATION_BUILDNUM),
				REGISTRATION_APARTMENT		= isnull(@REGISTRATION_APARTMENT,		REGISTRATION_APARTMENT),
				CURRENT_COUNTRY_CODE		= isnull(@CURRENT_COUNTRY_CODE,			CURRENT_COUNTRY_CODE),
				CURRENT_STATE_CODE			= isnull(@CURRENT_STATE_CODE,			CURRENT_STATE_CODE),
				CURRENT_CITY_CODE			= isnull(@CURRENT_CITY_CODE,			CURRENT_CITY_CODE),
				CURRENT_STREET				= isnull(@CURRENT_STREET,				CURRENT_STREET),
				CURRENT_BUILDNUM			= isnull(@CURRENT_BUILDNUM,				CURRENT_BUILDNUM),
				CURRENT_APARTMENT			= isnull(@CURRENT_APARTMENT,			CURRENT_APARTMENT),
				COMPANY_NAME				= isnull(@COMPANY_NAME,					COMPANY_NAME),
				COMPANY_PHONE				= isnull(@COMPANY_PHONE,				COMPANY_PHONE),
				POSITION					= isnull(@POSITION,						POSITION),
				MONTHLY_INCOME_CODE			= isnull(@MONTHLY_INCOME_CODE,			MONTHLY_INCOME_CODE),
				WORKING_EXPERIENCE_CODE		= isnull(@WORKING_EXPERIENCE_CODE,		WORKING_EXPERIENCE_CODE),
				FAMILY_STATUS_CODE			= isnull(@FAMILY_STATUS_CODE,			FAMILY_STATUS_CODE)
			where APPLICATION_USER_ID = @CUSTOMER_USER_ID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
