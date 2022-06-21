﻿if exists (select * from sys.objects where name='LOAN_GETTING_OPTION' and type='U')
	drop table dbo.LOAN_GETTING_OPTION
GO

CREATE TABLE dbo.LOAN_GETTING_OPTION (
	CODE 	char(1)			NOT NULL,
	NAME_AM	nvarchar(50)	NOT NULL,
	NAME_EN	varchar(50)		NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iLOAN_GETTING_OPTION1 ON dbo.LOAN_GETTING_OPTION(CODE)
GO