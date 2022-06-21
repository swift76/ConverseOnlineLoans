if exists (select * from sys.objects where name='sp_ChangeApplicationUserPasswordByID' and type='P')
	drop procedure dbo.sp_ChangeApplicationUserPasswordByID
GO

create procedure dbo.sp_ChangeApplicationUserPasswordByID(@ID					int,
														  @HASH					varchar(1000),
														  @PASSWORD_EXPIRY_DATE	date)
AS
	update dbo.APPLICATION_USER
	set HASH = @HASH,
		PASSWORD_EXPIRY_DATE = @PASSWORD_EXPIRY_DATE
	where ID = @ID
GO
