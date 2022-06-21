if exists (select * from sys.objects where name='CUSTOMER_USER_PASSWORD_RESET' and type='U')
	drop table dbo.CUSTOMER_USER_PASSWORD_RESET
GO

CREATE TABLE dbo.CUSTOMER_USER_PASSWORD_RESET(
    PROCESS_ID			uniqueidentifier NOT NULL PRIMARY KEY,
	LOGIN				varchar(20)		NOT NULL,
	HASH				varchar(1000)	NOT NULL,
	EXPIRES_ON			DATETIME		NOT NULL,
)
GO
