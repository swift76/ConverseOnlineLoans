if exists (select * from sys.objects where name='sp_AssignClientCode' and type='P')
	drop procedure dbo.sp_AssignClientCode
GO

create procedure dbo.sp_AssignClientCode(@APPLICATION_ID	uniqueidentifier,
										 @CLIENT_CODE		char(8))
AS
	update dbo.APPLICATION set CLIENT_CODE=@CLIENT_CODE
	where ID=@APPLICATION_ID
GO
