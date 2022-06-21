if exists (select * from sys.objects where name='sp_AuthenticateApplicationUser' and type='P')
	drop procedure dbo.sp_AuthenticateApplicationUser
GO

create procedure dbo.sp_AuthenticateApplicationUser(@LOGIN	varchar(50),
													@HASH	varchar(1000))
AS
	select ID,
		LOGIN,
		FIRST_NAME,
		LAST_NAME,
		EMAIL,
		CREATE_DATE,
		PASSWORD_EXPIRY_DATE,
		CLOSE_DATE,
		USER_STATE_ID,
		USER_ROLE_ID
	from dbo.APPLICATION_USER
	where upper(LOGIN) = upper(@LOGIN) and
		  HASH = @HASH and
		  USER_STATE_ID = 1
GO
