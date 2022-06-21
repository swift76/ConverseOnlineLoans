if exists (select * from sys.objects where name='ol0sp_GetApplicationPrint' and type='P')
	drop procedure dbo.ol0sp_GetApplicationPrint
GO

create procedure ol0sp_GetApplicationPrint(@APPLICATION_ID				uniqueidentifier,
										   @APPLICATION_PRINT_TYPE_ID	tinyint)
AS
	select CONTENT
	from AM0_APPLICATION_PRINT
	where APPLICATION_ID = @APPLICATION_ID and APPLICATION_PRINT_TYPE_ID = @APPLICATION_PRINT_TYPE_ID
GO
