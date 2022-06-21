if exists (select * from sys.objects where name='sp_LogClientIpAddress' and type='P')
	drop procedure dbo.sp_LogClientIpAddress
GO

create procedure dbo.sp_LogClientIpAddress(@IP_ADDRESS		varchar(20),
											@USER_ID		int = NULL,
											@USER_LOGIN		varchar(20) = NULL,
											@OPERATION_TYPE	varchar(30))
AS
	insert into dbo.IP_ADDRESS_LOG (IP_ADDRESS,USER_ID,USER_LOGIN,OPERATION_TYPE)
		values (@IP_ADDRESS,@USER_ID,@USER_LOGIN,@OPERATION_TYPE)
GO
