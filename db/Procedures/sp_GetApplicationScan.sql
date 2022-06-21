if exists (select * from sys.objects where name='sp_GetApplicationScan' and type='P')
	drop procedure dbo.sp_GetApplicationScan
GO

create procedure dbo.sp_GetApplicationScan(@APPLICATION_ID	uniqueidentifier)
AS
	SELECT APPLICATION_SCAN_TYPE_CODE, FILE_NAME, CREATION_DATE
	FROM   dbo.APPLICATION_SCAN
	WHERE  APPLICATION_ID = @APPLICATION_ID
GO