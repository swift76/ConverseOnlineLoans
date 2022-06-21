if exists (select * from sys.objects where name='sp_ChangeApplicationUserPassword' and type='P')
	drop procedure dbo.sp_ChangeApplicationUserPassword
GO

create procedure dbo.sp_ChangeApplicationUserPassword(@LOGIN				varchar(50),
													  @HASH					varchar(1000),
													  @PASSWORD_EXPIRY_DATE	date)
AS
	update dbo.APPLICATION_USER
	set HASH = @HASH,
		PASSWORD_EXPIRY_DATE = @PASSWORD_EXPIRY_DATE
	where LOGIN = @LOGIN
GO
