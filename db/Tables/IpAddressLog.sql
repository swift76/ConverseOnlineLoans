if exists (select * from sys.objects where name='IP_ADDRESS_LOG' and type='U')
	drop table dbo.IP_ADDRESS_LOG
GO

CREATE TABLE dbo.IP_ADDRESS_LOG
(
	ID				int				identity(1, 1)	NOT NULL,
	CREATION_DATE	datetime		NOT NULL default getdate(),
	IP_ADDRESS		varchar(20)		NOT NULL,
	USER_ID			int				NULL,
	USER_LOGIN		varchar(20)		NULL,
	OPERATION_TYPE	varchar(30)		NOT NULL,
)
GO

CREATE UNIQUE CLUSTERED INDEX iIP_ADDRESS_LOG1 ON IP_ADDRESS_LOG(ID)
GO
