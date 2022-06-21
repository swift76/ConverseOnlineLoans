if exists (select * from sys.objects where name='sp_GetNORQConfigData' and type='P')
	drop procedure dbo.sp_GetNORQConfigData
GO

create procedure dbo.sp_GetNORQConfigData
AS
	select URL,USER_NAME,USER_PASSWORD
	from dbo.SERVICE_CONFIGURATION
		where CODE='NORQ'
GO
