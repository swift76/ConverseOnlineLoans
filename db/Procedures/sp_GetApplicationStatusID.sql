if exists (select * from sys.objects where name='sp_GetApplicationStatusID' and type='P')
	drop procedure dbo.sp_GetApplicationStatusID
GO

create procedure dbo.sp_GetApplicationStatusID(@ID	uniqueidentifier)
AS
	select STATUS_ID
	from dbo.APPLICATION
	where ID = @ID
GO
