if exists (select * from sys.objects where name='sp_DeleteApplicationScan' and type='P')
	drop procedure dbo.sp_DeleteApplicationScan
GO

create procedure dbo.sp_DeleteApplicationScan(@APPLICATION_ID				uniqueidentifier,
											  @APPLICATION_SCAN_TYPE_CODE 	char(1))
AS
	delete from dbo.APPLICATION_SCAN
	where APPLICATION_ID = @APPLICATION_ID AND APPLICATION_SCAN_TYPE_CODE = @APPLICATION_SCAN_TYPE_CODE
GO
