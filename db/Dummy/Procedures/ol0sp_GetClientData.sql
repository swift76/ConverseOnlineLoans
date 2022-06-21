if exists (select * from sys.objects where name='ol0sp_GetClientData' and type='P')
	drop procedure dbo.ol0sp_GetClientData
GO

create procedure ol0sp_GetClientData(@CLICODE	nvarchar(8))
AS
	select
		'37455555555' as MobilePhone
GO
