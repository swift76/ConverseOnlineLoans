if exists (select * from sys.objects where name='sp_GetACRAConfigData' and type='P')
	drop procedure dbo.sp_GetACRAConfigData
GO

create procedure dbo.sp_GetACRAConfigData
AS
	select URL,USER_NAME,USER_PASSWORD
	from dbo.SERVICE_CONFIGURATION
		where CODE='ACRA'
GO
