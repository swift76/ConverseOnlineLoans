if exists (select * from sys.objects where name='sp_ChangeApplicationStatusByISN' and type='P')
	drop procedure dbo.sp_ChangeApplicationStatusByISN
GO

create procedure dbo.sp_ChangeApplicationStatusByISN(@ISN		int,
													 @STATUS_ID	tinyint)
AS
	if @STATUS_ID=10
		set @STATUS_ID=13

	update dbo.APPLICATION set
		STATUS_ID=@STATUS_ID,
		TO_BE_SYNCHRONIZED=0
	where ISN=@ISN
GO
