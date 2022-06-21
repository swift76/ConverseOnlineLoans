if exists (select * from sys.objects where name='APPLICATION_USER' and type='U')
	drop table dbo.APPLICATION_USER
GO

CREATE TABLE dbo.APPLICATION_USER (
	ID						int				identity(1,1),
	LOGIN					varchar(20)		NOT NULL,
	FIRST_NAME				nvarchar(50)	NOT NULL,
	LAST_NAME				nvarchar(50)	NOT NULL,
	HASH					varchar(1000)	NOT NULL,
	EMAIL					varchar(70)		NULL,
	CREATE_DATE				datetime		NOT NULL default getdate(),
	PASSWORD_EXPIRY_DATE	date			NULL,
	CLOSE_DATE				datetime		NULL,
	USER_STATE_ID			tinyint			NOT NULL,
	USER_ROLE_ID			tinyint			NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iAPPLICATION_USER1 ON dbo.APPLICATION_USER(ID)
GO

CREATE UNIQUE INDEX iAPPLICATION_USER2 ON dbo.APPLICATION_USER(USER_ROLE_ID, LOGIN)
GO
