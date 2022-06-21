if exists (select * from sys.objects where name='sp_LockApplicationByID' and type='P')
	drop procedure dbo.sp_LockApplicationByID
GO

create procedure dbo.sp_LockApplicationByID(@ID			uniqueidentifier,
											@STATUS_ID	tinyint)
AS
	select ID
		from dbo.APPLICATION with (UPDLOCK)
		where ID=@ID and STATUS_ID=@STATUS_ID
GO
