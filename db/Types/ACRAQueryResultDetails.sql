﻿if exists (select * from sys.types where name = 'ACRAQueryResultDetails')
	drop type dbo.ACRAQueryResultDetails
GO

CREATE TYPE dbo.ACRAQueryResultDetails AS TABLE
(
	STATUS					nvarchar(40)	NOT NULL,
	FROM_DATE				date			NOT NULL,
	TO_DATE					date			NOT NULL,
	TYPE					nvarchar(100)	NOT NULL,
	CUR						char(3)			NOT NULL,
	CONTRACT_AMOUNT			money			NOT NULL,
	DEBT					money			NOT NULL,
	PAST_DUE_DATE			date			NULL,
	RISK					nvarchar(40)	NOT NULL,
	CLASSIFICATION_DATE		date			NULL,
	INTEREST_RATE			money			NOT NULL,
	PLEDGE					nvarchar(1000)	NOT NULL,
	PLEDGE_AMOUNT			money			NOT NULL,
	OUTSTANDING_AMOUNT		money			NOT NULL,
	OUTSTANDING_PERCENT		money			NOT NULL,
	BANK_NAME				nvarchar(100)	NOT NULL,
	IS_GUARANTEE			bit				NOT NULL,
	DUE_DAYS_1				int				NOT NULL,
	DUE_DAYS_2				int				NOT NULL,
	DUE_DAYS_3				int				NOT NULL,
	DUE_DAYS_4				int				NOT NULL,
	DUE_DAYS_5				int				NOT NULL,
	LAST_REPAYMENT_DATE		date			NULL,
	SUM_OVERDUE_DAYS_Y1_Y1	int				NULL,
	MAX_OVERDUE_DAYS_Y1_Y1	int				NULL,
	SUM_OVERDUE_DAYS_Y1_Y2	int				NULL,
	MAX_OVERDUE_DAYS_Y1_Y2	int				NULL,
	SUM_OVERDUE_DAYS_Y1_Y3	int				NULL,
	MAX_OVERDUE_DAYS_Y1_Y3	int				NULL,
	SUM_OVERDUE_DAYS_Y1_Y4	int				NULL,
	MAX_OVERDUE_DAYS_Y1_Y4	int				NULL,
	SUM_OVERDUE_DAYS_Y1_Y5	int				NULL,
	MAX_OVERDUE_DAYS_Y1_Y5	int				NULL
)
GO
