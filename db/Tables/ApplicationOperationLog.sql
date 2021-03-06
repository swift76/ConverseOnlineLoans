if exists (select * from sys.objects where name='APPLICATION_OPERATION_LOG' and type='U')
	drop table dbo.APPLICATION_OPERATION_LOG
GO

CREATE TABLE dbo.APPLICATION_OPERATION_LOG (
	ID 						int IDENTITY(1, 1)	NOT NULL,
	DATE					datetime			NOT NULL default getdate(),
	CUSTOMER_USER_ID		int					NULL,
	SHOP_USER_ID			int					NULL,
	PARTNER_COMPANY_CODE	varchar(8)			NULL,
	LOAN_TYPE_ID			char(2)				NOT NULL,
	OPERATION_CODE			varchar(20)			NOT NULL,
	APPLICATION_ID			uniqueidentifier	NOT NULL,
	OPERATION_DETAILS		nvarchar(max)		NOT NULL
)
GO

CREATE CLUSTERED INDEX iAPPLICATION_OPERATION_LOG1 ON dbo.APPLICATION_OPERATION_LOG (ID)
GO

CREATE INDEX iAPPLICATION_OPERATION_LOG2 ON dbo.APPLICATION_OPERATION_LOG (DATE, CUSTOMER_USER_ID)
GO

CREATE INDEX iAPPLICATION_OPERATION_LOG3 ON dbo.APPLICATION_OPERATION_LOG (APPLICATION_ID)
GO

CREATE INDEX iAPPLICATION_OPERATION_LOG4 ON dbo.APPLICATION_OPERATION_LOG (DATE, SHOP_USER_ID)
GO

CREATE INDEX iAPPLICATION_OPERATION_LOG5 ON dbo.APPLICATION_OPERATION_LOG (DATE, PARTNER_COMPANY_CODE)
GO
