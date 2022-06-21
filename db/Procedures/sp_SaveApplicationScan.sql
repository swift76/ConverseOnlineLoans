if exists (select * from sys.objects where name='sp_SaveApplicationScan' and type='P')
	drop procedure dbo.sp_SaveApplicationScan
GO

create procedure dbo.sp_SaveApplicationScan(@APPLICATION_ID				uniqueidentifier,
											@APPLICATION_SCAN_TYPE_CODE	char(1),
											@FILE_NAME					nvarchar(250),
											@CONTENT					varbinary(max))
AS
	if not exists (select *
				   from dbo.APPLICATION_SCAN
				   where APPLICATION_ID = @APPLICATION_ID AND APPLICATION_SCAN_TYPE_CODE = @APPLICATION_SCAN_TYPE_CODE)
       insert into dbo.APPLICATION_SCAN
			(APPLICATION_ID, APPLICATION_SCAN_TYPE_CODE, FILE_NAME, CONTENT)
       values
			(@APPLICATION_ID, @APPLICATION_SCAN_TYPE_CODE, @FILE_NAME, @CONTENT)
    else
		update dbo.APPLICATION_SCAN
		set	CONTENT = @CONTENT, FILE_NAME = @FILE_NAME
		where APPLICATION_ID = @APPLICATION_ID AND APPLICATION_SCAN_TYPE_CODE = @APPLICATION_SCAN_TYPE_CODE
GO
