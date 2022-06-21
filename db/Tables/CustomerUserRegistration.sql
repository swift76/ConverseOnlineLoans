if exists (select * from sys.objects where name='CUSTOMER_USER_REGISTRATION' and type='U')
	drop table dbo.CUSTOMER_USER_REGISTRATION
GO

CREATE TABLE dbo.CUSTOMER_USER_REGISTRATION(
	PROCESS_ID			uniqueidentifier NOT NULL PRIMARY KEY,
	VERIFICATION_CODE	char(4)			NOT NULL, -- TODO: why not HASH here???
	FIRST_NAME			nvarchar(20)	NOT NULL,
	LAST_NAME			nvarchar(20)	NOT NULL,
	MOBILE_PHONE		varchar(20)		NOT NULL,
	EMAIL				varchar(70)		NULL,
	SOCIAL_CARD_NUMBER	char(10)		NOT NULL,
	HASH				varchar(1000)	NOT NULL,
)
GO
