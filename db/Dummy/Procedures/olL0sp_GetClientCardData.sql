if exists (select * from sys.objects where name='ol0sp_GetClientCardData' and type='P')
	drop procedure dbo.ol0sp_GetClientCardData
GO

create procedure ol0sp_GetClientCardData(@CLICODE	char(8),
										 @CARDNUM	char(16),
										 @EXPIRY	smalldatetime)
AS
	select
		'JOHN SMITH' as EmbossedName,
		'37455555555' as MobilePhone
	where @CARDNUM='9051190400000020'
GO
