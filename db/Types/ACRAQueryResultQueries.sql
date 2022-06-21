if exists (select * from sys.types where name = 'ACRAQueryResultQueries')
	drop type dbo.ACRAQueryResultQueries
GO

CREATE TYPE dbo.ACRAQueryResultQueries AS TABLE
(
	DATE		date			NOT NULL,
	BANK_NAME	nvarchar(100)	NOT NULL,
	REASON		nvarchar(100)	NULL
)
GO
