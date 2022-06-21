if exists (select * from sys.objects where name='sp_StartCustomerUserPasswordReset' and type='P')
	drop procedure dbo.sp_StartCustomerUserPasswordReset
GO

create procedure dbo.sp_StartCustomerUserPasswordReset (
	@PROCESS_ID				uniqueidentifier,
	@LOGIN					varchar(15),
	@HASH					varchar(1000)
)

AS
	insert into dbo.CUSTOMER_USER_PASSWORD_RESET
		(PROCESS_ID, LOGIN, HASH, EXPIRES_ON)
	values
		(@PROCESS_ID, @LOGIN, @HASH, DATEADD(MINUTE, 30, GETDATE()))
GO
