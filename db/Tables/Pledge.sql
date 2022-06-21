if exists (select * from sys.objects where name='PLEDGE' and type='U')
	drop table PLEDGE
GO

CREATE TABLE PLEDGE (
	ID				int identity(1,1)	NOT NULL,
	APPLICATION_ID	uniqueidentifier	NOT NULL,
	CLIENT_CODE		char(8)				NULL,
	AMOUNT			money				NOT NULL,
	CURRENCY		char(3)				NOT NULL,
	DETAILS			nvarchar(100)		NULL,
	RATIO			money				NULL,
	PLEDGE_NAME		nvarchar(100)		NOT NULL,
	PRICE			money				NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iPLEDGES1 ON dbo.PLEDGE(ID)
GO
CREATE INDEX iPLEDGES2 on dbo.PLEDGE(APPLICATION_ID)
GO
