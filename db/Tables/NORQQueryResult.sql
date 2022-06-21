﻿if exists (select * from sys.objects where name='NORQ_QUERY_RESULT' and type='U')
	drop table dbo.NORQ_QUERY_RESULT
GO

CREATE TABLE dbo.NORQ_QUERY_RESULT(
	QUERY_DATE							datetime			NOT NULL default getdate(),
	APPLICATION_ID						uniqueidentifier	NOT NULL,
	FIRST_NAME							nvarchar(40)		NOT NULL,
	LAST_NAME							nvarchar(40)		NOT NULL,
	PATRONYMIC_NAME						nvarchar(40)		NOT NULL,
	BIRTH_DATE							date				NOT NULL,
	GENDER								bit					NOT NULL,
	DISTRICT							nvarchar(40)		NOT NULL,
	COMMUNITY							nvarchar(40)		NOT NULL,
	STREET								nvarchar(100)		NOT NULL,
	BUILDING							nvarchar(40)		NOT NULL,
	APARTMENT							nvarchar(40)		NOT NULL,
	FEE									money				NOT NULL,
	NON_BIOMETRIC_PASSPORT_NUMBER		char(9)				NOT NULL,
	NON_BIOMETRIC_PASSPORT_ISSUE_DATE	date				NOT NULL,
	NON_BIOMETRIC_PASSPORT_EXPIRY_DATE	date				NOT NULL,
	NON_BIOMETRIC_PASSPORT_ISSUED_BY	char(3)				NOT NULL,
	BIOMETRIC_PASSPORT_NUMBER			char(9)				NOT NULL,
	BIOMETRIC_PASSPORT_ISSUE_DATE		date				NOT NULL,
	BIOMETRIC_PASSPORT_EXPIRY_DATE		date				NOT NULL,
	BIOMETRIC_PASSPORT_ISSUED_BY		char(3)				NOT NULL,
	ID_CARD_NUMBER						char(9)				NOT NULL,
	ID_CARD_ISSUE_DATE					date				NOT NULL,
	ID_CARD_EXPIRY_DATE					date				NOT NULL,
	ID_CARD_ISSUED_BY					char(3)				NOT NULL,
	SOCIAL_CARD_NUMBER					char(10)			NOT NULL,
	RESPONSE_XML						nvarchar(max)		NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX iNORQ_QUERY_RESULT1 ON dbo.NORQ_QUERY_RESULT (APPLICATION_ID)
GO