if exists (select * from sys.objects where name='APPLICATION' and type='U')
	drop table dbo.APPLICATION
GO

CREATE TABLE dbo.APPLICATION(
	-- columns identified before scoring
	CREATION_DATE				datetime			NOT NULL,
	SOURCE_ID					tinyint				NOT NULL,
	ID 							uniqueidentifier	NOT NULL,
	LOAN_TYPE_ID				char(2)				NOT NULL,
	STATUS_ID					tinyint				NOT NULL,
	CUSTOMER_USER_ID			int					NULL,
	SHOP_USER_ID				int					NULL,
	PARTNER_COMPANY_CODE		varchar(8)			NULL,
	INITIAL_AMOUNT				money				NULL,
	CURRENCY_CODE				char(3)				NULL,
	REFUSAL_REASON				nvarchar(100)		NULL,
	MANUAL_REASON				nvarchar(100)		NULL,

	FIRST_NAME_AM				nvarchar(20)		NULL,
	LAST_NAME_AM				nvarchar(20)		NULL,
	PATRONYMIC_NAME_AM			nvarchar(20)		NULL,
	BIRTH_DATE					date				NULL,
	SOCIAL_CARD_NUMBER			char(10)			NULL,
	DOCUMENT_TYPE_CODE			char(1)				NULL,
	DOCUMENT_NUMBER				char(9)				NULL,
	DOCUMENT_GIVEN_BY			char(3)				NULL,
	DOCUMENT_GIVEN_DATE			date				NULL,
	DOCUMENT_EXPIRY_DATE		date				NULL,
	ORGANIZATION_ACTIVITY_CODE	char(2)				NULL,

	PRODUCT_TOTAL_AMOUNT		money				NULL,
	PREPAID_AMOUNT				money				NULL,
	CUSTOMER_STATUS_ID			tinyint				NULL, -- TODO: Added for Converse, add corresponding property in backend
	IS_DATA_COMPLETE			bit					NULL,
	CLIENT_CODE					char(8)				NULL,
	ISN							int					NULL,
	HAS_BANK_CARD				bit					NULL,
	TO_BE_SYNCHRONIZED			bit					NOT NULL default 0,
	NORQ_TRY_COUNT				tinyint				NOT NULL default 0,
	ACRA_TRY_COUNT				tinyint				NOT NULL default 0,

	-- columns identified after scoring
	FINAL_AMOUNT				money				NULL,
	PERIOD_TYPE_CODE			char(2)				NULL,
	INTEREST					money				NULL,
	REPAY_DAY					tinyint				NULL,
	COMMUNICATION_TYPE_CODE		char(1)				NULL,

	LOAN_GETTING_OPTION_CODE	char(1)				NULL,
	EXISTING_CARD_CODE			char(16)			NULL,
	IS_NEW_CARD					bit					NULL,
	CREDIT_CARD_TYPE_CODE		char(3)				NULL, -- TODO: this is CARD_TYPE_CODE
	IS_CARD_DELIVERY			bit					NULL,
	CARD_DELIVERY_ADDRESS		nvarchar(150)		NULL,
	BANK_BRANCH_CODE			char(3)				NULL,
	MOBILE_PHONE_2				varchar(20)			NULL,
	CARD_RECOVERY_CODE			varchar(20)			NULL,

	FIRST_NAME_EN				varchar(20)			NULL,
	LAST_NAME_EN				varchar(20)			NULL,
	BIRTH_PLACE_CODE			char(2)				NULL,
	CITIZENSHIP_CODE			char(2)				NULL,
	EMAIL						varchar(70)			NULL,
	REGISTRATION_COUNTRY_CODE	char(2)				NULL,
	REGISTRATION_STATE_CODE		char(3)				NULL,
	REGISTRATION_CITY_CODE		char(9)				NULL,
	REGISTRATION_STREET			nvarchar(150)		NULL,
	REGISTRATION_BUILDNUM		nvarchar(30)		NULL,
	REGISTRATION_APARTMENT		nvarchar(30)		NULL,
	CURRENT_COUNTRY_CODE		char(2)				NULL,
	CURRENT_STATE_CODE			char(3)				NULL,
	CURRENT_CITY_CODE			char(9)				NULL,
	CURRENT_STREET				nvarchar(150)		NULL,
	CURRENT_BUILDNUM			nvarchar(30)		NULL,
	CURRENT_APARTMENT			nvarchar(30)		NULL,
	LOAN_TEMPLATE_CODE			char(4)				NULL,
	OVERDRAFT_TEMPLATE_CODE		char(4)				NULL,

	GOODS_RECEIVING_CODE		char(1)				NULL,
	GOODS_DELIVERY_ADDRESS		nvarchar(150)		NULL,

	MOBILE_PHONE_1				varchar(20)			NULL,
	FIXED_PHONE					varchar(20)			NULL,

	COMPANY_NAME				nvarchar(40)		NULL,
	COMPANY_PHONE				varchar(20)			NULL,
	POSITION					nvarchar(50)		NULL,
	MONTHLY_INCOME_CODE			char(1)				NULL,
	WORKING_EXPERIENCE_CODE		char(1)				NULL,
	FAMILY_STATUS_CODE			char(1)				NULL,
	SHOP_CODE					char(4)				NULL,
	PRODUCT_CATEGORY_CODE		char(2)				NULL,
	SHOP_SEQUENCE_NUMBER		varchar(20)			NULL,


	EFFECTIVE_INTEREST			money				NULL,
	MONTHLY_PAID_AMOUNT			money				NULL,
	FIRST_PAID_INTEREST			money				NULL,
	FIRST_PAID_PRINCIPAL_AMOUNT	money				NULL,
	TOTAL_PAID_AMOUNT			money				NULL,
	TOTAL_PAID_INTEREST			money				NULL,

	IS_ARBITRAGE_CHECKED		bit					NULL,

	PRINT_EXISTS				bit					NULL,
	AGREEMENT_NUMBER			nvarchar(14)		NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iAPPLICATION1 ON dbo.APPLICATION (ID)
GO

CREATE INDEX iAPPLICATION2 ON dbo.APPLICATION (TO_BE_SYNCHRONIZED, CREATION_DATE)
GO
